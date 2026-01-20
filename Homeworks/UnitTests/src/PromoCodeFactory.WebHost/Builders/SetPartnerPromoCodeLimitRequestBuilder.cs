using PromoCodeFactory.WebHost.Models;
using System;

namespace PromoCodeFactory.WebHost.Builders
{
    public class SetPartnerPromoCodeLimitRequestBuilder
    {
        private readonly SetPartnerPromoCodeLimitRequest _request = new();

        public static SetPartnerPromoCodeLimitRequestBuilder Builder() => new();

        public SetPartnerPromoCodeLimitRequestBuilder WithLimit(int limit)
        {
            _request.Limit = limit;
            return this;
        }

        public SetPartnerPromoCodeLimitRequestBuilder WithEndDate(DateTime date)
        {
            _request.EndDate = date;
            return this;
        }

        public SetPartnerPromoCodeLimitRequest Build() => _request;
    }
}
