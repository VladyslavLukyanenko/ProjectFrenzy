using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrenzyBot.Structures.FlashSale;

namespace FrenzyBot
{
    public static class Menu
    {

        public static async Task<List<Flashsale>> DisplayProductMenu(bool ActiveOnly = true)
        {

            List<Flashsale> FlashSales = await Endpoints.GetFlashSales();
            List<Flashsale> CorrectFlashSales = new List<Flashsale>();
            DateTime now = DateTime.UtcNow;
            Console.WriteLine("[ ID ] - ITEM NAME");



            foreach (Flashsale Sale in FlashSales)
            {

                if ((!Sale.Hidden && !Sale.SoldOut) || Sale.Password == "onboarding_sale")
                {
                    var DropType = Sale.Dropzone.Count == 0 ? "Worldwide Drop" : "Local Dropzone";
                    
                    if((Sale.StartedAt.UtcDateTime - now).Seconds > 0)

                    {
                        var DelayTime = Sale.StartedAt.UtcDateTime - now;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[{Sale.Id}] - {Sale.Title} | Drop Type: {DropType} | Store: {Sale.Shop.Name} | Pickup: {Sale.Pickup} | {(Sale.ProductsCount == 0 ? "Warning, Product has no products!" : "")} | Available in: {DelayTime.Days} days {DelayTime.Hours} hours {DelayTime.Minutes} minutes {DelayTime.Seconds} seconds");
                        Sale.Upcoming = true;
                        CorrectFlashSales.Add(Sale);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{Sale.Id}] - {Sale.Title} | Drop Type: {DropType}  | Store: {Sale.Shop.Name} | Pickup: {Sale.Pickup} {(Sale.ProductsCount == 0 ? "Warning, Product has no products!" : "")}");
                        Sale.Upcoming = false;
                        CorrectFlashSales.Add(Sale);
                    }
                }
                else
                {
                    if (!ActiveOnly)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{Sale.Id}] - {Sale.Title} | Store: {Sale.Shop.Name} | Pickup: {Sale.Pickup} ");
                        CorrectFlashSales.Add(Sale);
                    }
                }

                Console.ResetColor();
            }

            return CorrectFlashSales;
        }
    }
}