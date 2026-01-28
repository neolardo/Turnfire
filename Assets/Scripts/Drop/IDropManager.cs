using System;
using System.Collections.Generic;

public interface IDropManager 
{
    event Action AllPackagesLanded;
    void TrySpawnPackages();
    IEnumerable<IPackage> GetAllAvailablePackages();

}
