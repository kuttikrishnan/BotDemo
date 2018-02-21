using HotelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HotelBot.Dialogs
{
	//[LuisModel("e73128cf-c14b-4b31-9cf8-196c771183e5", "253cdcc8eb464c25817edccb8554077e")]
    [LuisModel("88f38907-28c4-4834-9bab-e0febde12e95", "a7722545196d4b34b25bc21d233b76a6")]
    [Serializable]
	public class LUISDialog : LuisDialog<RoomReservation>
	{
		private readonly BuildFormDelegate<RoomReservation> ReserveRoom;

		public LUISDialog(BuildFormDelegate<RoomReservation> reserveRoom)
		{
			this.ReserveRoom = reserveRoom;
		}


		[LuisIntent("")]
		public async Task None(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("I'm sorry I don't know what you mean.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Greeting")]
		public async Task Greeting(IDialogContext context, LuisResult result)
		{
			context.Call(new GreetingDialog(), Callback);
		}

		private async Task Callback(IDialogContext context, IAwaitable<object> result)
		{
			context.Wait(MessageReceived);
		}

		[LuisIntent("ReserveRoom")]
		public async Task RoomReservation(IDialogContext context, LuisResult result)
		{
			var enrollmentForm = new FormDialog<RoomReservation>(new RoomReservation(), this.ReserveRoom, FormOptions.PromptInStart);
			context.Call<RoomReservation>(enrollmentForm, Callback);
		}

		[LuisIntent("QueryAmenities")]
		public async Task QueryAmenities(IDialogContext context, LuisResult result)
		{
			foreach (var entity in result.Entities.Where(Entity => Entity.Type == "Amenity"))
			{
				var value = entity.Entity.ToLower();
				if (value == "pool" || value == "gym" || value == "wifi" || value == "towels")
				{
					await context.PostAsync("Yes we have that!");
					context.Wait(MessageReceived);
					return;
				}
				else
				{
					await context.PostAsync("I'm sorry we don't have that.");
					context.Wait(MessageReceived);
					return;
				}
			}
			await context.PostAsync("I'm sorry we don't have that.");
			context.Wait(MessageReceived);
			return;
		}

	}
}