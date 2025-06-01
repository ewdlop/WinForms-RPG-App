using System.Collections.Generic;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for location and world navigation management
    /// </summary>
    public interface ILocationManager : IBaseManager
    {
        /// <summary>
        /// Current location where the player is
        /// </summary>
        Location CurrentLocation { get; }

        /// <summary>
        /// Key of the current location
        /// </summary>
        string CurrentLocationKey { get; }

        /// <summary>
        /// All available locations in the world
        /// </summary>
        Dictionary<string, Location> AllLocations { get; }

        /// <summary>
        /// All available locations in the world (alias for AllLocations)
        /// </summary>
        Dictionary<string, Location> Locations { get; }

        /// <summary>
        /// Current player for location tracking
        /// </summary>
        Player CurrentPlayer { get; }

        /// <summary>
        /// Set the current player for location tracking
        /// </summary>
        /// <param name="player">Player to track</param>
        void SetCurrentPlayer(Player player);

        /// <summary>
        /// Load all locations from data files
        /// </summary>
        /// <returns>True if locations were loaded successfully</returns>
        bool LoadLocations();

        /// <summary>
        /// Move player to a specific location
        /// </summary>
        /// <param name="locationKey">Key of the location to move to</param>
        /// <returns>True if movement was successful</returns>
        bool MoveToLocation(string locationKey);

        /// <summary>
        /// Move player in a specific direction from current location
        /// </summary>
        /// <param name="direction">Direction to move (north, south, east, west, etc.)</param>
        /// <returns>True if movement was successful</returns>
        bool MoveInDirection(string direction);

        /// <summary>
        /// Get available exits from the current location
        /// </summary>
        /// <returns>Dictionary of direction -> location key</returns>
        Dictionary<string, string> GetAvailableExits();

        /// <summary>
        /// Get available exits as a formatted string
        /// </summary>
        /// <returns>Formatted string of available exits</returns>
        string GetExitsDescription();

        /// <summary>
        /// Check if a location exists
        /// </summary>
        /// <param name="locationKey">Key of the location to check</param>
        /// <returns>True if location exists</returns>
        bool LocationExists(string locationKey);

        /// <summary>
        /// Get a location by its key
        /// </summary>
        /// <param name="locationKey">Key of the location</param>
        /// <returns>Location object or null if not found</returns>
        Location GetLocation(string locationKey);

        /// <summary>
        /// Add a new location to the world
        /// </summary>
        /// <param name="location">Location to add</param>
        /// <returns>True if location was added successfully</returns>
        bool AddLocation(Location location);

        /// <summary>
        /// Remove a location from the world
        /// </summary>
        /// <param name="locationKey">Key of the location to remove</param>
        /// <returns>True if location was removed successfully</returns>
        bool RemoveLocation(string locationKey);

        /// <summary>
        /// Check for random encounters at the current location
        /// </summary>
        /// <returns>Enemy to fight, or null if no encounter</returns>
        Enemy CheckForRandomEncounter();

        /// <summary>
        /// Get items available at the current location
        /// </summary>
        /// <returns>List of items at current location</returns>
        List<Item> GetLocationItems();

        /// <summary>
        /// Pick up an item from the current location
        /// </summary>
        /// <param name="item">Item to pick up</param>
        /// <returns>True if item was picked up successfully</returns>
        bool PickUpItem(Item item);

        /// <summary>
        /// Drop an item at the current location
        /// </summary>
        /// <param name="item">Item to drop</param>
        /// <returns>True if item was dropped successfully</returns>
        bool DropItem(Item item);

        /// <summary>
        /// Get NPCs at the current location
        /// </summary>
        /// <returns>List of NPC names at current location</returns>
        List<string> GetLocationNPCs();

        /// <summary>
        /// Get enemies at the current location
        /// </summary>
        /// <returns>List of enemies at current location</returns>
        List<Enemy> GetLocationEnemies();

        /// <summary>
        /// Mark current location as visited
        /// </summary>
        void MarkCurrentLocationVisited();

        /// <summary>
        /// Get all visited locations
        /// </summary>
        /// <returns>List of visited location keys</returns>
        List<string> GetVisitedLocations();

        /// <summary>
        /// Get location description including items, NPCs, and exits
        /// </summary>
        /// <returns>Full description of current location</returns>
        string GetLocationDescription();

        /// <summary>
        /// Search the current location for hidden items or secrets
        /// </summary>
        /// <returns>List of found items or empty list</returns>
        List<Item> SearchLocation();

        /// <summary>
        /// Get the distance between two locations
        /// </summary>
        /// <param name="fromKey">Starting location key</param>
        /// <param name="toKey">Destination location key</param>
        /// <returns>Distance in steps, or -1 if no path exists</returns>
        int GetDistanceBetweenLocations(string fromKey, string toKey);

        /// <summary>
        /// Find the shortest path between two locations
        /// </summary>
        /// <param name="fromKey">Starting location key</param>
        /// <param name="toKey">Destination location key</param>
        /// <returns>List of location keys representing the path</returns>
        List<string> FindPath(string fromKey, string toKey);

        /// <summary>
        /// Get random encounter chance for current location
        /// </summary>
        /// <returns>Encounter chance as percentage (0.0 to 1.0)</returns>
        double GetEncounterChance();

        /// <summary>
        /// Set random encounter chance for a location
        /// </summary>
        /// <param name="locationKey">Location to modify</param>
        /// <param name="chance">Encounter chance (0.0 to 1.0)</param>
        void SetEncounterChance(string locationKey, double chance);
    }
} 