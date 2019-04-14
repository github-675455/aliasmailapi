using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using AliasMailApi.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using AliasMailApi.Repository;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AliasMailApi.Configuration;
using System.Text;
using System.Security.Cryptography;
using AutoMapper;
using AliasMailApi.Models.DTO;
using AliasMailApi.Extensions;
using System.Collections.Generic;
using AliasMailApi.Interfaces;
using AliasMailApi.Models.Enum;

namespace AliasMailApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly MessageContext _context;
        private readonly IOptions<AppOptions> _options;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _accessor;
        public MessageService(MessageContext context, IOptions<AppOptions> options, IMapper mapper, IHttpContextAccessor accessor)
        {
            _context = context;
            _options = options;
            _mapper = mapper;
            _accessor = accessor;
        }

        public async Task<BaseResponse<MailgunMessage>> update(MailgunMessageRequest messageRequest)
        {
            var message = _mapper.Map<MailgunMessage>(messageRequest);

            MailgunMessage messageFound = null;

            var finalResponse = new BaseResponse<MailgunMessage>();

            IList<ApiError> errors = new List<ApiError>();

            var filter = false;

            if (messageRequest.Token.NotEmpty())
            {
                messageFound = await _context.MailgunMessages.FirstOrDefaultAsync(e => e.Token == messageRequest.Token);
                if (messageFound == null)
                    errors.Add(new ApiError { description = "Token not found." });

                filter = true;
            }

            if (messageRequest.MessageId.NotEmpty())
            {
                messageFound = await _context.MailgunMessages.FirstOrDefaultAsync(e => e.MessageId == messageRequest.MessageId);
                if (messageFound == null)
                    errors.Add(new ApiError { description = "MessageId not found." });

                filter = true;
            }

            if (messageFound != null)
            {
                _context.Attach(messageFound).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                finalResponse.Success = true;
                finalResponse.Data = messageFound;
            }

            if (!filter)
            {
                errors.Add(new ApiError { description = "inform either MessageId or Token to delete." });
            }

            if (errors.Any())
            {
                finalResponse.Errors = errors.ToArray();
            }

            return finalResponse;
        }

        public async Task<BaseResponse<MailgunMessage>> delete(MailgunMessageRequest messageRequest)
        {
            var message = _mapper.Map<MailgunMessage>(messageRequest);

            MailgunMessage messageFound = null;

            var finalResponse = new BaseResponse<MailgunMessage>();

            IList<ApiError> errors = new List<ApiError>();

            var filter = false;

            if (messageRequest.Token.NotEmpty())
            {
                messageFound = await _context.MailgunMessages.FirstOrDefaultAsync(e => e.Token == messageRequest.Token);
                if (messageFound == null)
                    errors.Add(new ApiError { description = "Token not found." });

                filter = true;
            }

            if (messageRequest.MessageId.NotEmpty())
            {
                messageFound = await _context.MailgunMessages.FirstOrDefaultAsync(e => e.MessageId == messageRequest.MessageId);
                if (messageFound == null)
                    errors.Add(new ApiError { description = "MessageId not found." });

                filter = true;
            }

            if (messageFound != null)
            {
                _context.Remove(messageFound);
                await _context.SaveChangesAsync();
                finalResponse.Success = true;
                finalResponse.Data = messageFound;
            }

            if (!filter)
            {
                errors.Add(new ApiError { description = "inform either MessageId or Token to delete." });
            }

            if (errors.Any())
            {
                finalResponse.Errors = errors.ToArray();
            }

            return finalResponse;
        }

        public async Task<BaseResponse<MailgunMessage>> create(MailgunMessageRequest messageRequest)
        {

            messageRequest.RemoteIpAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            var message = _mapper.Map<MailgunMessage>(messageRequest);

            var tokenFound = await _context.MailgunMessages.FirstOrDefaultAsync(e => e.Token == message.Token);

            if (tokenFound != null)
                return new BaseResponse<MailgunMessage> { Errors = new ApiError[] { new ApiError { description = "Token already used." } } };

            var mailgunApiToken = Encoding.ASCII.GetBytes(_options.Value.mailgunApiToken);
            var hash = new HMACSHA256(mailgunApiToken);
            if (BitConverter.ToString(hash.ComputeHash(Encoding.ASCII.GetBytes(message.Timestamp + message.Token))).Replace("-", "").ToLower().Equals(message.Signature))
            {
                message.Valid = true;
            }

            message.Validated = DateTime.Now;

            await _context.AddAsync(message);
            await _context.SaveChangesAsync();

            return new BaseResponse<MailgunMessage>
            {
                Success = true,
                Data = message
            };
        }

        public async Task<BaseResponse<MailgunMessage>> get(string id)
        {
            var messageFound = await _context.MailgunMessages.FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

            return new BaseResponse<MailgunMessage>()
            {
                Success = true,
                Data = messageFound
            };
        }

        public async Task<List<MailgunMessage>> getNextForProcessing()
        {
            var result = await (from messages in _context.MailgunMessages
                                join mails in _context.Mails
                                on messages.Id equals mails.BaseMessageId into joinMessagesWithMails
                                from mails in joinMessagesWithMails.DefaultIfEmpty()
                                where messages.Valid == true
                                && (mails == null ||
                                (
                                    (mails.JobStats == JobStats.Pending || mails.JobStats == JobStats.Error) &&
                                    mails.Retries > 0 &&
                                    mails.Source == 0 &&
                                    mails.NextRetry < DateTime.Now)
                                )
                                select messages).AsNoTracking().ToListAsync();

            return result;
        }
    }
}