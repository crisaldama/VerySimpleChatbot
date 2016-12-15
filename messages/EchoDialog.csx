using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types

//using sforce;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;
  
    
    public void resetValues()
    {
        // will reset all values to blank
        count = 1;
       
    }

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        if (message.Text == "reset")
        {
            PromptDialog.Confirm(
                context,
                AfterResetAsync,
                "¿Estás seguro de volver a empezar?",
                "¡No me he enterado!",
                promptStyle: PromptStyle.Auto);
        }
        else if (message.Text == "OK" || message.Text == "De acuerdo" || message.Text == "vale" )
        {
               PromptDialog.Confirm(
                context,
                AfterOKAsync,
                "Por favor valida que tus datos son correctos y te daremos una respuesta en menos de 24 horas<br>Comprar coche nuevo",
                "¡No me he enterado!",
                promptStyle: PromptStyle.Auto);
        }
        else
        {
            this.count++;

            switch(count)
            {
                case 2: await context.PostAsync($"Para pedir un crédito puedes consultar nuestra web en www.bancosabadell.com, pero si quieres contestar a unas preguntas yo puedo ayudarte más rápido. ¿Para qué es el crédito y qué cantidad necesitas?");
                    break;
                case 3: await context.PostAsync($"Ya veo, necesitas 5.000€ para un coche nuevo. ¿Es cierto que llevas ya 2 años trabajando?");
                    break;
                case 4: await context.PostAsync($"¿Te parece bien un crédito a 3 años?");
                    break;
                case 5: await context.PostAsync($"En principio la operación parece viable. ¿Quieres que iniciémos los trámites?");
                    break;
                default:
                    break;

            }

            context.Wait(MessageReceivedAsync);
        }
    }

    public async Task AfterOKAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            //CreateNewOpportunity in Sales Cloud
            // QuickstartApiSample sample = new QuickstartApiSample();
            // sample.run();
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=verysimplebot;AccountKey=g/znXlaP4eYeKI57YZP0IGJlZKb8/1accLODo+wCwxuAHH9daIB0fmAL7IwdUARbtxobH3pXdMKZVY+zsJCukw==");

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference("myqueue");

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage("Create Opp");
            queue.AddMessage(message);


            await context.PostAsync("Oportunidad creada correctamente");
        }
        else
        {
            await context.PostAsync("No hay problema si no se está seguro. ¿Te puedo ayudar en algo más?");
        }
        context.Wait(MessageReceivedAsync);
    }
    
    public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            //reset all variables
            this.resetValues();
            await context.PostAsync("¿Te puedo ayudar en algo m&aacute;s?");
        }
        else
        {
            await context.PostAsync("No he tocado nada");
        }
        context.Wait(MessageReceivedAsync);
    }
}
