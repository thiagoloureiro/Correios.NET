﻿using System.Threading.Tasks;
using Correios.NET.Models;

namespace Correios.NET
{
    public interface IServices
    {
        Task<Package> GetPackageTrackingAsync(string packageCode);
        Package GetPackageTracking(string packageCode);

        Task<Address> GetAddressAsync(string zipCode);
        Address GetAddress(string zipCode);
    }
}