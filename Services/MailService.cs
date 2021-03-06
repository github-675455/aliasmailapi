using System.Net.Mail;
using System.Threading.Tasks;
using AliasMailApi.Models;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AliasMailApi.Interfaces;
using AutoMapper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using AliasMailApi.Extensions;
using Microsoft.Extensions.Logging;
using AliasMailApi.Models.Enum;
using aliasmailapi.Factory;

namespace AliasMailApi.Services
{
    public class MailService : IMailService
    {
        private readonly MessageContext _context;
        private readonly IMessageService _messageService;
        private readonly IDomainService _domainService;
        private readonly IMailboxService _mailboxService;
        private readonly IMapper _mapper;
        private readonly IMailgunAttachment _mailgunAttachment;
        private readonly ILogger _logger;

        public MailService(
            MessageContext context,
            IMessageService messageService,
            IDomainService domainService,
            IMapper mapper,
            IMailgunAttachment mailgunAttachment,
            IMailboxService mailboxService,
            ILogger<MailService> logger)
        {
            _context = context;
            _messageService = messageService;
            _domainService = domainService;
            _mapper = mapper;
            _mailgunAttachment = mailgunAttachment;
            _mailboxService = mailboxService;
            _logger = logger;
        }

        public async Task SetMailError(Mail mail, string errorMessage)
        {
            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                mail.JobStats = JobStats.Error;

                if (errorMessage.Length > 4096)
                    errorMessage = errorMessage.Substring(0, 4096);
                else
                    mail.ErrorMessage = errorMessage;

                mail.ErrorDate = DateTime.Now;
                mail.Retries = mail.Retries - 1;
                mail.NextRetry = DateTime.Now.AddMinutes(10);

                _context.Mails.Update(mail);
                await _context.SaveChangesAsync();

                transaction.Commit();
            }
        }

        private async Task<BaseResponse<BaseMessage>> import(BaseMessage message)
        {
            var response = BaseResponseFactory<BaseMessage>.CreateDefaultBaseResponse();

            using(var transaction = await _context.Database.BeginTransactionAsync())
            {
                if (message == null)
                {
                    _logger.LogInformation("Parameter is null");
                    response.Errors.Add(new ApiError { description = "Parameter is null" });
                    return response;
                }

                if (!message.Valid)
                {
                    _logger.LogInformation("Message {Id} is not valid by provider, so we cannot process it", message.Id);
                    response.Errors.Add(new ApiError { description = "Message is not valid by provider, so we cannot process it" });
                    return response;
                }

                if (message is MailgunMessage)
                {
                    var mailgunMessage = (MailgunMessage)message;

                    Mail newMail = null;

                    try
                    {
                        newMail = await _context.Mails
                            .AsNoTracking()
                            .FirstOrDefaultAsync(e => e.BaseMessageId == message.Id);

                        if (newMail == null)
                        {
                            newMail = new Mail(message);
                            await _context.Mails.AddAsync(newMail);
                            await _context.SaveChangesAsync();
                        }

                        if (newMail.Retries <= 0)
                        {
                            _logger.LogError("Too many reprocessed");
                            return response;
                        }

                        bool recipientEmpty = mailgunMessage.Recipient != null ? mailgunMessage.Recipient.Equals(string.Empty) : true;

                        bool toEmpty = mailgunMessage.To != null ?
                            mailgunMessage.To.Equals(string.Empty) : true;

                        if(recipientEmpty && toEmpty)
                        {
                            _logger.LogInformation("recipient and to email headers are empty");
                            response.Errors.Add(new ApiError { description = "recipient and to email headers are empty" });
                            return response;
                        }

                        var toRecipient = !recipientEmpty ? new MailAddress(mailgunMessage.Recipient) : new MailAddress(mailgunMessage.To);
                        var domainFound = await _domainService.get(toRecipient.Host);

                        if (domainFound == null)
                        {
                            var errorMessage = string.Format("message id: {0} the domain {1} was not found", message.Id, toRecipient.Host);
                            await SetMailError(newMail, errorMessage);
                            _logger.LogError(errorMessage);
                            response.Errors.Add(new ApiError { description = "Domain not found" });
                            return response;
                        }

                        var mailboxFound = await _mailboxService.GetMailbox(toRecipient.Address);

                        if (mailboxFound == null)
                        {
                            var newMailbox = _mailboxService.CreateDefaultMailbox(toRecipient, domainFound);
                            mailboxFound = await _mailboxService.CreateMailbox(newMailbox);
                        }

                        var mail = _mapper.Map<Mail>(mailgunMessage);

                        mail.Id = newMail.Id;
                        mail.BaseMessageId = newMail.BaseMessageId;

                        _context.Entry(newMail).State = EntityState.Detached;

                        if (mail.Attachments.NotEmpty())
                        {
                            var jsonAttachments = mail.Attachments.Replace("\\\"", "\"");
                            var attachments = JsonConvert.DeserializeObject<MailAttachment[]>(jsonAttachments);
                            ICollection<Task<MailAttachment>> getFiles = new List<Task<MailAttachment>>();

                            attachments.ToList().ForEach(async e =>
                            {
                                e = await _mailgunAttachment.get(e);
                                e.MailId = mail.Id;
                                e.Id = Guid.NewGuid();
                                await _context.AddAsync(e);
                            });

                            await _context.SaveChangesAsync();

                            mail.MailAttachmentsJobStatus = JobStats.Done;
                        }

                        mail.ErrorMessage = string.Empty;
                        mail.ErrorDate = null;
                        mail.NextRetry = null;

                        mail.JobStats = JobStats.Done;
                        mail.MailAttachmentsJobStatus = JobStats.Done;

                        _context.Mails.Update(mail);

                        await _context.SaveChangesAsync();

                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();

                        var errorMessage = string.Format("{0} - {1}", exception.Message, exception.StackTrace);
                        await SetMailError(newMail, errorMessage);
                        _logger.LogError(errorMessage);
                        response.Errors.Add(new ApiError { description = errorMessage });
                    }
                }
            }

            return response;
        }

        public Task<BaseResponse<BaseMessage>> process(BaseMessage message)
        {
            return import(message);
        }
    }
}