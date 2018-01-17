using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Collections.Generic;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;
        public enum Topics
        {
            AreMormonsChristian,
            god,
            trinity,
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(GetTopic);
        }

        public virtual async Task GetTopic(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;
            var descriptions = new string[] { "Are Mormons Christian?", "What do Mormons believe about God?", "What do Mormons believe about the Trinity?" };

            PromptDialog.Choice(
                context: context,
                resume: ChoiceReceivedAsync,
                options: (IEnumerable<Topics>)Enum.GetValues(typeof(Topics)),
                prompt: "Please select a topic about the LDS Church to learn more:",
                retry: "Topic not available. Please try again.",
                promptStyle: PromptStyle.Auto,
                descriptions: descriptions
                );
        }

        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<Topics> activity)
        {
            Topics response = await activity;
            await context.PostAsync($"Your made a chocice of {response.ToString()}");

            //context.Call<object>(new AnnualPlanDialog(response.ToString()), ChildDialogComplete);

        }



        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            if (count == 1)
            {
                await context.PostAsync("Hi there, I am Elder Bot.  I can answer questions about the Church of Jesus Christ of Latter Day Saints or commonly called the Mormons.  What would you like to know?");
            }
            else
            {
                await context.PostAsync($"{this.count++}: You typed: {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}