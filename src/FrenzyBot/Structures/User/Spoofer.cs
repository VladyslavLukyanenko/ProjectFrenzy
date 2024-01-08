using FrenzyBot.Structures.FlashSale;
using System;
using System.Collections.Generic;

namespace FrenzyBot.Structures.User
{
    public class Spoofer
    {
        private double LeftMost, RightMost, TopMost, LowerMost;
        private Random random = new Random();

        public Spoofer(List<Dropzone> Zone)
        {
            LeftMost = Zone[0].Lat;
            RightMost = Zone[0].Lat;
            TopMost = Zone[0].Lng;
            LowerMost = Zone[0].Lng;

            for (int i = 1; i < Zone.Count - 1; i++)
            {
                if (Zone[i].Lng > Zone[i - 1].Lng)
                    TopMost = Zone[i].Lng;
                else
                    LowerMost = Zone[i].Lng;

                if (Zone[i].Lat > Zone[i - 1].Lat)
                    RightMost = Zone[i].Lat;
                else
                    LeftMost = Zone[i].Lat;
            }
        }

        public Dropzone Spoof()
        {
            return new Dropzone
            {
                Lat = random.NextDouble() * (RightMost - LeftMost) + LeftMost,
                Lng = random.NextDouble() * (TopMost - LowerMost) + LowerMost
            };
        }
    }
}
