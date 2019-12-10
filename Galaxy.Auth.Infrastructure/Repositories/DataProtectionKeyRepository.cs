using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Galaxy.Auth.Core.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Galaxy.Auth.Infrastructure.Repositories
{
    public class DataProtectionKeyRepository : IXmlRepository
    {
        private readonly AuthDbContext _db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db">EF Data Context containing the DataProtectionKey table</param>
        public DataProtectionKeyRepository(AuthDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all XML Elements from the database
        /// </summary>
        /// <returns>Readonly Collection of XElements</returns>
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return new ReadOnlyCollection<XElement>(_db.DataProtectionKeys.Select(k => XElement.Parse(k.XmlData))
                .ToList());
        }

        /// <summary>
        /// Stores an XML Element to the database
        /// </summary>
        /// <param name="element">XML Element</param>
        /// <param name="friendlyName">Friendly Name</param>
        public void StoreElement(XElement element, string friendlyName)
        {
            if (element == null)
                throw new ArgumentException("Element is null", nameof(element));
            
            var entity = _db.DataProtectionKeys.SingleOrDefault(k => k.FriendlyName == friendlyName);
            if (null != entity)
            {
                entity.XmlData = element.ToString();
                _db.DataProtectionKeys.Update(entity);
            }
            else
            {
                _db.DataProtectionKeys.Add(new DataProtectionKey
                {
                    FriendlyName = friendlyName,
                    XmlData = element.ToString()
                });
            }

            _db.SaveChanges();
        }
    }

}