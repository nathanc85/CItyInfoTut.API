using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CItyInfoTut.API.Entities;

namespace CItyInfoTut.API.Services
{
    public class CityInfoRepository: ICityInfoRepository
    {
        private CityInfoContext _ctx;

        public CityInfoRepository(CityInfoContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int cityId) {
            return _ctx.Cities.Any(c => c.Id == cityId);
        }

        public void DeletePointOfInterest(PointOfInterest poi)
        {
            _ctx.PointsOfInterest.Remove(poi);
        }

        public IEnumerable<City> GetCities()
        {
            return _ctx.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest) {
                return _ctx.Cities.Include(c => c.PointsOfInterest)
                           .Where(c => c.Id == cityId).FirstOrDefault();
            }
            return _ctx.Cities.FirstOrDefault(c => c.Id == cityId);
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _ctx.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                       .FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _ctx.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
        }

        public bool Save()
        {
            return (_ctx.SaveChanges() >= 0);
        }
    }
}
