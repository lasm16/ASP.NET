using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.WebHost.Builders
{
    public class PartnerBuilder
    {
        private readonly Partner _partner = new();

        private PartnerBuilder()
        {
            _partner = new Partner
            {
                PartnerLimits = new List<PartnerPromoCodeLimit>()
            };
        }

        public static PartnerBuilder Builder() => new();

        public PartnerBuilder WithId(Guid id)
        {
            _partner.Id = id;
            return this;
        }

        public PartnerBuilder WithName(string name)
        {
            _partner.Name = name;
            return this;
        }

        public PartnerBuilder WithActive(bool isActive)
        {
            _partner.IsActive = isActive;
            return this;
        }

        public PartnerBuilder WithNumberIssuedPromocodes(int count)
        {
            _partner.NumberIssuedPromoCodes = count;
            return this;
        }

        /// <summary>
        /// Добавляет лимит партнёра. PartnerId автоматически привязывается к родителю.
        /// </summary>
        public PartnerBuilder AddLimit(
            Guid? id = null,
            DateTime? createDate = null,
            DateTime? cancelDate = null,
            DateTime? endDate = null,
            int limit = 1)
        {
            if (endDate == null)
                throw new ArgumentException("EndDate must be provided for a PartnerPromoCodeLimit.");

            var promoLimit = new PartnerPromoCodeLimit
            {
                Id = id ?? Guid.NewGuid(),
                Partner = _partner,
                PartnerId = _partner.Id,
                CreateDate = createDate ?? DateTime.UtcNow,
                CancelDate = cancelDate,
                EndDate = endDate.Value,
                Limit = limit
            };

            _partner.PartnerLimits.Add(promoLimit);
            return this;
        }

        public Partner Build()
        {
            if (_partner.Id == Guid.Empty)
                _partner.Id = Guid.NewGuid();

            // Убедимся, что все лимиты имеют правильный PartnerId
            foreach (var limit in _partner.PartnerLimits)
            {
                limit.Partner = _partner;
                limit.PartnerId = _partner.Id;
            }

            return _partner;
        }
    }
}
