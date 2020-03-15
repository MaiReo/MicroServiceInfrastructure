
using Core.Web.Messages;
using Core.Web.EFCore;
using Core.Messages;
using Core.Messages.Bus;
using Core.PersistentStore.Repositories;
using Core.PersistentStore.Repositories.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Web.Controllers
{
    public class HomeController : Controller
    {
        public async ValueTask<IActionResult> Index(
            [FromServices]IRepository<WebEntity> repository,
            [FromServices]IRepository<WebEntity, int> repositoryWithKey,
            [FromServices]IAsyncRepository<WebEntity> asyncRepository,
            [FromServices]IAsyncRepository<WebEntity, int> asyncRepositoryWithKey,
            [FromServices]IMessageBus messageBus,
            [FromServices]IMessageConverter messageConverter)
        {
            var repositoryStatus = "OK";
            var messageBusStatus = "OK";
            var messageConverterStatus = "OK";
            try
            {
                var hashSet = new HashSet<DbContext>
            {
                repository.GetDbContext(),
                repositoryWithKey.GetDbContext(),
                asyncRepository.GetDbContext(),
                asyncRepositoryWithKey.GetDbContext()
            };
                hashSet.Single();
            }
            catch (Exception)
            {
                repositoryStatus = "Error";
            }
            try
            {
                var message = new WebTestMessage();
                var descriptor = new RichMessageDescriptor("", nameof(WebTestMessage).ToLowerInvariant());
                await messageBus.OnMessageReceivedAsync(message, descriptor);
            }
            catch (Exception)
            {
                messageBusStatus = "Error";
            }
            var testMessage = new WebTestMessage
            {
                TestMessage = "debug"
            };
            var serializedMessageString = messageConverter.SerializeString(testMessage);

            if (serializedMessageString == "{}")
            {
                messageConverterStatus = "Error";
            }

            return Json(new
            {
                RepositoryStatus = repositoryStatus,
                MessageBusStatus = messageBusStatus,
                MessageConverterStatus = messageConverterStatus,
            });
        }

        public async ValueTask<IActionResult> DebugMessage([FromQuery]string message, [FromServices]IMessageBus messageBus)
        {
            var msg = new WebTestMessage() { TestMessage = message };
            await messageBus.PublishAsync(msg);
            return Json(new { message });
        }


    }
}
