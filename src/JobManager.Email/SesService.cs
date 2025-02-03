using Amazon.Runtime.Internal.Util;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using JobManager.Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace JobManager.Email;

public class SesService(ILogger<SesService> logger,  IAmazonSimpleEmailService sesClient) : IEmailService
{
    public async Task SendEmailAsync(string subject, string destination, string message)
    {
        try
        {
            await sesClient.SendEmailAsync(
                new SendEmailRequest
                {
                    Destination = new Destination
                    {
                        ToAddresses = [destination]
                    },
                    Message = new Message
                    {
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = message
                            },
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = message
                            }
                        },
                        Subject = new Content
                        {
                            Charset = "UTF-8",
                            Data = subject
                        }
                    },
                    Source = "atendimento@fiap.com.br"
                });
        }
        catch (Exception ex)
        {
            logger.LogError("SendEmailAsync failed with exception: " + ex.Message);
        }
    }
}