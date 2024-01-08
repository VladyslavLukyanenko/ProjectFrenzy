using System;
using System.Collections.Generic;
using ProjectFrenzy.Core.Model.FlashSale;

namespace ProjectFrenzy.Core.Model
{
    public class Spoofer
    {
        private readonly double _leftMost;
        private readonly double _rightMost;
        private readonly double _topMost;
        private readonly double _lowerMost;
        private readonly Random _random = new Random((int) DateTime.Now.Ticks);

        public Spoofer(List<Dropzone> zone)
        {
            _leftMost = zone[0].Lat;
            _rightMost = zone[0].Lat;
            _topMost = zone[0].Lng;
            _lowerMost = zone[0].Lng;

            for (int i = 1; i < zone.Count; i++)
            {
                if (zone[i].Lng > zone[i - 1].Lng)
                    _topMost = zone[i].Lng;
                else
                    _lowerMost = zone[i].Lng;

                if (zone[i].Lat > zone[i - 1].Lat)
                    _rightMost = zone[i].Lat;
                else
                    _leftMost = zone[i].Lat;
            }
        }

        public Dropzone Spoof()
        {
            return new Dropzone
            {
                Lat = _random.NextDouble() * (_rightMost - _leftMost) + _leftMost,
                Lng = _random.NextDouble() * (_topMost - _lowerMost) + _lowerMost
            };
        }
    }
}
