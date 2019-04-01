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

namespace AliasMailApi.Services
{
    public class MailboxService : IMailboxService
    {
        private readonly MessageContext _context;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IMailgunAttachment _mailgunAttachment;
        public MailboxService(
            MessageContext context,
            IMessageService messageService,
            IMapper mapper,
            IMailgunAttachment mailgunAttachment){
            _context = context;
            _messageService = messageService;
            _mapper = mapper;
            _mailgunAttachment = mailgunAttachment;
        }

        public async Task<Domain> GetDomain(string domain){
            return await _context.Domains.FirstOrDefaultAsync(e => e.Name == domain);
        }
        public async Task<Mailbox> GetMailbox(string email){
            return await _context.Mailboxes.FirstOrDefaultAsync(e => e.Email == email);
        }
        public async Task<Mailbox> CreateMailbox(Mailbox mail){
            await _context.Mailboxes.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        public async Task import(BaseMessage message)
        {
            if(message == null){
                return;
            }

            if(!message.Valid)
            {
                return;
            }
            
            if(message is MailgunMessage)
            {
                var mailgunMessage = (MailgunMessage)message;
                var toEmail = new MailAddress(mailgunMessage.To);
                
                var domainFound = await GetDomain(toEmail.Host);

                if(domainFound == null){
                    //mark to delete mailgunmMessage
                    return;
                }

                var mailboxFound = await GetMailbox(toEmail.Address);
                
                if(mailboxFound == null)
                {
                    var newMailbox = new Mailbox{
                        Email = toEmail.Address,
                        StoreQuantity = 1,
                        Description = toEmail.DisplayName,
                        DomainId = domainFound.Id,
                        Domain = domainFound
                    };
                    mailboxFound = await CreateMailbox(newMailbox);
                }

                try{

                    var mailFound = await _context.Mails.FirstOrDefaultAsync(e => e.BaseMessageId == mailgunMessage.Id);

                    if(mailFound != null)
                    {
                        return;
                    }
                    
                    var mail = _mapper.Map<Mail>(mailgunMessage);

                    using(var transaction = _context.Database.BeginTransaction())
                    {
                        await _context.Mails.AddAsync(mail);
                        await _context.SaveChangesAsync();

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

                            await _context.SaveChangesAsync();
                        }
                        
                        transaction.Commit();
                    }
                }
                catch(Exception exception)
                {
                    mailgunMessage.Error = true;
                    mailgunMessage.ErrorMessage = string.Format("{0} - {1}",exception.Message, exception.StackTrace).Substring(0, 4096);
                    _context.MailgunMessages.Update(mailgunMessage);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}