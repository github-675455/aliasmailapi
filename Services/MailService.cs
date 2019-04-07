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
using Newtonsoft.Json.Linq;
using System;
using AliasMailApi.Extensions;
using Microsoft.Extensions.Logging;
using AliasMailApi.Models.Enum;

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
            ILogger<MailService> logger){
            _context = context;
            _messageService = messageService;
            _domainService = domainService;
            _mapper = mapper;
            _mailgunAttachment = mailgunAttachment;
            _mailboxService = mailboxService;
            _logger = logger;
        }

        public async Task SetMailError(Mail mail, string errorMessage) {

            mail.Error = true;
            if(errorMessage.Length > 4096)
                errorMessage = errorMessage.Substring(0, 4096);
            
            mail.ErrorMessage = errorMessage;
            mail.ErrorDate = DateTime.Now;
            mail.Retries = mail.Retries - 1;
            mail.NextRetry = DateTime.Now.AddMinutes(10);

            _context.Mails.Update(mail);
            await _context.SaveChangesAsync();
        }

        public async Task<BaseResponse<BaseMessage>> import(BaseMessage message)
        {
            var response = new BaseResponse<BaseMessage>();
            if(message == null){
                _logger.LogInformation("Parameter is null");
                response.Errors.Add(new ApiError{ description = "Parameter is null" });
                return response;
            }
            
            if(!message.Valid)
            {
                _logger.LogInformation("Message {Id} is not valid by provider, so we cannot process it", message.Id);
                response.Errors.Add(new ApiError{ description = "Message is not valid by provider, so we cannot process it" });
                return response;
            }
            
            if(message is MailgunMessage)
            {
                Mail newMail = new Mail(message);

                try
                {
                    var mailFound = await _context.Mails.FirstOrDefaultAsync(e => e.BaseMessageId == message.Id);

                    if(mailFound == null)
                    {
                        await _context.Mails.AddAsync(newMail);
                        await _context.SaveChangesAsync();
                    }
                    else{
                        newMail = mailFound;
                    }

                    if(newMail.Retries <= 0){
                        _logger.LogError("Too many reprocessed");
                        return response;
                    }

                    var mailgunMessage = (MailgunMessage)message;

                    var toEmail = new MailAddress(mailgunMessage.To);
                    
                    var domainFound = await _domainService.get(toEmail.Host);

                    if(domainFound == null){
                        //mark to delete mailgunmMessage
                        var errorMessage = string.Format("message id: {0} the domain {1} was not found", message.Id, toEmail.Host);
                        await SetMailError(newMail, errorMessage);
                        _logger.LogError(errorMessage);
                        response.Errors.Add(new ApiError{ description = "Domain not found" });
                        return response;
                    }

                    var mailboxFound = await _mailboxService.GetMailbox(toEmail.Address);
                    
                    if(mailboxFound == null)
                    {
                        var newMailbox = _mailboxService.CreateDefaultMailbox(toEmail, domainFound);
                        mailboxFound = await _mailboxService.CreateMailbox(newMailbox);
                    }

                    var mail = _mapper.Map<Mail>(mailgunMessage);

                    mail.Id = newMail.Id;
                    mail.BaseMessageId = newMail.BaseMessageId;

                    newMail = mail;

                    using(var transaction = _context.Database.BeginTransaction())
                    {
                        if(mail.Attachments.NotEmpty())
                        {
                            var jsonAttachments = mail.Attachments.Replace("\\\"", "\"");
                            var attachments = JsonConvert.DeserializeObject<MailAttachment[]>(jsonAttachments);
                            ICollection<Task<MailAttachment>> getFiles = new List<Task<MailAttachment>>();

                            attachments.ToList().ForEach(e => {
                                getFiles.Add(_mailgunAttachment.get(e));
                                e.MailId = mail.Id;
                                e.url = string.Empty;
                            });
                            await Task.WhenAll(getFiles.ToArray());

                            getFiles.ToList().ForEach(async attachment => await _context.MailAttachments.AddAsync(attachment.Result));

                            mail.MailAttachmentsJobStatus = JobStats.Done;
                        }

                        mail.JobStatus = JobStats.Done;

                        _context.Mails.Update(mail);

                        await _context.SaveChangesAsync();
                        
                        transaction.Commit();
                    }
                }
                catch(Exception exception)
                {
                    var errorMessage = string.Format("{0} - {1}", exception.Message, exception.StackTrace);
                    await SetMailError(newMail, errorMessage);
                    _logger.LogError(errorMessage);
                }
            }

            return response;
        }
    }
}