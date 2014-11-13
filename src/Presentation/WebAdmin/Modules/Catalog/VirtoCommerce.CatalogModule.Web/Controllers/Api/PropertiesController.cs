﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.CatalogModule.Web.Converters;
using VirtoCommerce.CatalogModule.Services;
using moduleModel = VirtoCommerce.CatalogModule.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Controllers.Api
{
    public class PropertiesController : ApiController
    {
        private readonly IPropertyService _propertyService;
		private readonly ICategoryService _categoryService;
	
        public PropertiesController(IPropertyService propertyService, ICategoryService categoryService)
        {
            _propertyService = propertyService;
			_categoryService = categoryService;
		
        }

        // GET api/properties/propertyvalues
        [HttpGet]
		[ResponseType(typeof(webModel.PropertyValue[]))]
        public IHttpActionResult GetPropertyValues(string propertyId, string keyword = null)
        {
			var retVal = new List<webModel.PropertyValue>();
			//Need return propValue because it more convenient for ui
			var dictValues = _propertyService.SearchDictionaryValues(propertyId, keyword);
			foreach(var dictValue in dictValues)
			{
				var propValue = new webModel.PropertyValue
				{
					Value = dictValue.Value,
					ValueId = dictValue.Id,
					LanguageCode = dictValue.LanguageCode
				};
				retVal.Add(propValue);
			}
			return Ok(retVal.ToArray());
        }

        // GET: api/properties/
		[ResponseType(typeof(webModel.Property))]
        public IHttpActionResult Get(string propertyId)
        {
			var property = _propertyService.GetById(propertyId);
			if (property == null)
			{
				return NotFound();
			}
			var retVal = property.ToWebModel();
		    retVal.IsManageable = true;
			return Ok(retVal);
        }

        // GET: api/properties/getnewproperty
        [HttpGet]
		[ResponseType(typeof(webModel.Property))]
        public IHttpActionResult GetNewProperty(string categoryId)
        {
			var category = _categoryService.GetById(categoryId);
			var retVal = new webModel.Property
			{
				Id = Guid.NewGuid().ToString(),
				IsNew = true,
				CategoryId = categoryId,
				Category = category.ToWebModel(),
				CatalogId = category.CatalogId,
				Catalog = category.Catalog.ToWebModel(),
				Name = "new property",
				Type = webModel.PropertyType.Category,
				ValueType = webModel.PropertyValueType.ShortText,
				DictionaryValues = new List<webModel.PropertyDictionaryValue>(),
				Attributes = new List<webModel.PropertyAttribute>()
			};
		
            return Ok(retVal);
        }

        // POST: api/properties/post
        [HttpPost]
        [ResponseType(typeof(void))]
		public IHttpActionResult Post(webModel.Property property)
        {
			var moduleProperty = property.ToModuleModel();
		
			if (property.IsNew)
			{
				_propertyService.Create(moduleProperty);
			}
			else
			{
				_propertyService.Update(new moduleModel.Property[] { moduleProperty });
			}

		    return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/properties/id
		[HttpDelete]
        [ResponseType(typeof(void))]
        public IHttpActionResult Delete(string id)
        {
            _propertyService.Delete(new string[] { id });
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}