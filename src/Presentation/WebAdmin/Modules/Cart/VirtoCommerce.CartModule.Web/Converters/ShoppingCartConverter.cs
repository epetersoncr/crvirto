﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.Foundation.Money;
using coreModel = VirtoCommerce.Domain.Cart.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
	public static class ShoppingCartConverter
	{
		public static webModel.ShoppingCart ToWebModel(this coreModel.ShoppingCart cart)
		{
			var retVal = new webModel.ShoppingCart();
			retVal.InjectFrom(cart);

			retVal.Currency = cart.Currency;
			if(cart.BillingAddresses != null)
				retVal.BillingAddresses = cart.BillingAddresses.Select(x => x.ToWebModel()).ToList();
			if(cart.ShippingAddresses != null)
				retVal.ShippingAddresses = cart.ShippingAddresses.Select(x => x.ToWebModel()).ToList();
			if(cart.Items != null)
				retVal.Items = cart.Items.Select(x => x.ToWebModel()).ToList();
			if(cart.Payments != null)
				retVal.Payments = cart.Payments.Select(x => x.ToWebModel()).ToList();
			if(cart.Shipments != null)
				retVal.Shipments = cart.Shipments.Select(x => x.ToWebModel()).ToList();
			if(cart.Discounts != null)
				retVal.Discounts = cart.Discounts.Select(x => x.ToWebModel()).ToList();

			return retVal;
		}

		public static coreModel.ShoppingCart ToCoreModel(this webModel.ShoppingCart cart)
		{
			var retVal = new coreModel.ShoppingCart();
			retVal.InjectFrom(cart);

		
			if(retVal.IsTransient())
			{
				retVal.Id = Guid.NewGuid().ToString();
			}

			retVal.Currency = cart.Currency;
					
			if(cart.BillingAddresses != null)
				retVal.BillingAddresses = cart.BillingAddresses.Select(x => x.ToCoreModel()).ToList();
			if(cart.ShippingAddresses != null)
				retVal.ShippingAddresses = cart.ShippingAddresses.Select(x => x.ToCoreModel()).ToList();
			if(cart.Items != null)
				retVal.Items = cart.Items.Select(x => x.ToCoreModel()).ToList();
			if(cart.Payments != null)
				retVal.Payments = cart.Payments.Select(x => x.ToCoreModel()).ToList();
			if(cart.Shipments != null)
				retVal.Shipments = cart.Shipments.Select(x => x.ToCoreModel()).ToList();
			if(cart.Discounts != null)
				retVal.Discounts = cart.Discounts.Select(x => x.ToCoreModel()).ToList();

			return retVal;
		}


	}
}