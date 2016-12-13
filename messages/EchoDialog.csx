using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;
    protected int age = -1;
    protected bool isEmployed = false;
    protected string name = "";
    protected string lastName = "";
    
    public void resetValues()
    {
        // will reset all values to blank
        int count = 1;
        int age = -1;
        bool isEmployed = false;
        string name = "";
        string lastName = ""; 
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
        else if (message.Text == "De acuerdo")
        {
               PromptDialog.Confirm(
                context,
                AfterOKAsync,
                "Por favor confirme que está de acuerdo en que un especialista contacte con usted por teléfono",
                "¡No me he enterado!",
                promptStyle: PromptStyle.Auto);
        }
        else
        {
            await context.PostAsync($"{this.count++}: You said {message.Text}");
            context.Wait(MessageReceivedAsync);
        }
    }

    public async Task AfterOKAsync(IDialogContext context, IAwaitable<bool> argument)
    {
        var confirm = await argument;
        if (confirm)
        {
            //CreateNewOpportunity in Sales Cloud
            this.count = 1;
            await context.PostAsync("Oportunidad creada");
        }
        else
        {
            await context.PostAsync("No hay problema si no está seguro");
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
            await context.PostAsync("Valores reseteados ok");
        }
        else
        {
            await context.PostAsync("No he tocado nada");
        }
        context.Wait(MessageReceivedAsync);
    }
}
