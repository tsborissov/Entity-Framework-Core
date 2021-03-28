using RealEstates.Models;
using System.Collections.Generic;

namespace RealEstates.Services
{
    public interface IPropertiesService
    {
        void Add(string district, int price, int floor, int totalFloors, int size,
            int yardSize, int year, string propertyType, string buildingType);


        IEnumerable<Property> Search(int minPrice, int maxPrice, int minSize, int maxSize);

    }
}