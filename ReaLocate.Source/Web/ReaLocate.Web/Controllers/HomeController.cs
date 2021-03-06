﻿namespace ReaLocate.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using Infrastructure.Mapping;
    using ViewModels;
    using Services.Data.Contracts;
    using System;
    using Microsoft.AspNet.Identity;
    public class HomeController : BaseController
    {
        private const int ItemsPerPage = 10;
        private readonly IRealEstatesService realEstateService;

        public HomeController(IRealEstatesService realEstateService)
        {
            this.realEstateService = realEstateService;
        }

        public ActionResult GetAllRealEstatesList(string id)
        {
            int page;
            if (id == string.Empty || id == null)
            {
                page = 1;
            }
            else
            {
                page = int.Parse(id);
            }

            var allItemsCount = this.realEstateService.GetAll().Count();
            var totalPages = (int)Math.Ceiling(allItemsCount / (decimal)ItemsPerPage);
            var itemsToSkip = (page - 1) * ItemsPerPage;

            var estates = this.realEstateService.GetAllForPaging(itemsToSkip, ItemsPerPage)
                         .To<DetailsRealEstateViewModel>().ToList();

            var coordinates = this.GetCoordinates(estates);

            var indexView = new IndexMapAndGridViewModel
            {
                MapsCoordinates = coordinates,
                Estates = estates,
                TotalPages = totalPages,
                CurrentPage = page,
                UserId = this.User.Identity.GetUserId()
            };
            return this.View(indexView);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var city = this.HttpContext.Request.QueryString["City"];
            var country = this.HttpContext.Request.QueryString["Country"];

            IEnumerable<DetailsRealEstateViewModel> result;

            if (city != null || country != null)
            {
                result = this.realEstateService.GetAll()
                                     .Where(r => r.City.Contains(city) && r.Country.Contains(country))
                                     .OrderByDescending(r => r.CreatedOn)
                                     .To<DetailsRealEstateViewModel>()
                                     .ToList();

            }
            else
            {
                result =
               this.Cache.Get(
                   "latest20RealEstates",
                   () => this.realEstateService.GetAll()
                                     .OrderByDescending(r => r.CreatedOn)
                                     .Take(20)
                                     .To<DetailsRealEstateViewModel>()
                                     .ToList(),
                   15 * 60);
            }

            foreach (var realEstate in result)
            {
                realEstate.EncodedId = this.realEstateService.EncodeId(realEstate.Id);
            }

            return this.View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SearchedRealEstateViewModel model)
        {
            var result = this.realEstateService
                .GetAll()
                .Where(r => r.City.Contains(model.City) || r.Country.Contains(model.Country))
                .To<DetailsRealEstateViewModel>()
                 .ToList();

            return View(result);
        }

        public ActionResult Chat()
        {
            return View();
        }

        private List<CoordinateViewModel> GetCoordinates(List<DetailsRealEstateViewModel> estates)
        {
            var coordinates = new List<CoordinateViewModel>();

            foreach (var estate in estates)
            {
                estate.EncodedId = this.realEstateService.EncodeId(estate.Id);
                var coordinate = new CoordinateViewModel
                {
                    Address = estate.Address,
                    EncodedId = estate.EncodedId,
                    GeoLat = estate.Latitude,
                    GeoLong = estate.Longitude
                };
                coordinates.Add(coordinate);
            }

            return coordinates;
        }
    }
}
