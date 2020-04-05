namespace NuGetPacksCLI.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface INugetManager
    {
        /// <summary>
        /// Get all versions of package
        /// </summary>
        /// <param name="n">Package name</param>
        /// <returns></returns>
        void ListVersions(string n);
    }
}