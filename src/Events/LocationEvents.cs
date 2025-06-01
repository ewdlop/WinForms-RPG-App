using System.Collections.Generic;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event published when player moves to a new location
    /// </summary>
    public class LocationChangedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Location PreviousLocation { get; set; }
        public Location NewLocation { get; set; }
        public string MovementDirection { get; set; }
        public bool IsFirstVisit { get; set; }

        public override int Priority => 10; // High priority

        public LocationChangedEvent(Player player, Location previousLocation, Location newLocation, string movementDirection = "", bool isFirstVisit = false)
        {
            Player = player;
            PreviousLocation = previousLocation;
            NewLocation = newLocation;
            MovementDirection = movementDirection;
            IsFirstVisit = isFirstVisit;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when a random encounter occurs
    /// </summary>
    public class RandomEncounterEvent : GameEvent
    {
        public Player Player { get; set; }
        public Location Location { get; set; }
        public Enemy Enemy { get; set; }
        public double EncounterChance { get; set; }
        public bool WasTriggered { get; set; }

        public override int Priority => 9; // High priority

        public RandomEncounterEvent(Player player, Location location, Enemy enemy, double encounterChance, bool wasTriggered = true)
        {
            Player = player;
            Location = location;
            Enemy = enemy;
            EncounterChance = encounterChance;
            WasTriggered = wasTriggered;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when player picks up an item from a location
    /// </summary>
    public class ItemPickedUpEvent : GameEvent
    {
        public Player Player { get; set; }
        public Location Location { get; set; }
        public Item Item { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public ItemPickedUpEvent(Player player, Location location, Item item, bool wasSuccessful, string failureReason = "")
        {
            Player = player;
            Location = location;
            Item = item;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when player drops an item at a location
    /// </summary>
    public class ItemDroppedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Location Location { get; set; }
        public Item Item { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public ItemDroppedEvent(Player player, Location location, Item item, bool wasSuccessful, string failureReason = "")
        {
            Player = player;
            Location = location;
            Item = item;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when player searches a location
    /// </summary>
    public class LocationSearchedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Location Location { get; set; }
        public List<Item> ItemsFound { get; set; }
        public bool WasSuccessful { get; set; }
        public string SearchResult { get; set; }

        public LocationSearchedEvent(Player player, Location location, List<Item> itemsFound, bool wasSuccessful, string searchResult = "")
        {
            Player = player;
            Location = location;
            ItemsFound = itemsFound ?? new List<Item>();
            WasSuccessful = wasSuccessful;
            SearchResult = searchResult;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when movement is attempted but fails
    /// </summary>
    public class MovementFailedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Location CurrentLocation { get; set; }
        public string AttemptedDirection { get; set; }
        public string AttemptedLocationKey { get; set; }
        public string FailureReason { get; set; }

        public MovementFailedEvent(Player player, Location currentLocation, string attemptedDirection, string attemptedLocationKey, string failureReason)
        {
            Player = player;
            CurrentLocation = currentLocation;
            AttemptedDirection = attemptedDirection;
            AttemptedLocationKey = attemptedLocationKey;
            FailureReason = failureReason;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when locations are loaded from data
    /// </summary>
    public class LocationsLoadedEvent : GameEvent
    {
        public int LocationCount { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> LoadedLocationKeys { get; set; }

        public override int Priority => 15; // Highest priority

        public LocationsLoadedEvent(int locationCount, bool wasSuccessful, string errorMessage = "", List<string> loadedLocationKeys = null)
        {
            LocationCount = locationCount;
            WasSuccessful = wasSuccessful;
            ErrorMessage = errorMessage;
            LoadedLocationKeys = loadedLocationKeys ?? new List<string>();
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when a new location is added to the world
    /// </summary>
    public class LocationAddedEvent : GameEvent
    {
        public Location Location { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public LocationAddedEvent(Location location, bool wasSuccessful, string failureReason = "")
        {
            Location = location;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when a location is removed from the world
    /// </summary>
    public class LocationRemovedEvent : GameEvent
    {
        public string LocationKey { get; set; }
        public Location RemovedLocation { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public LocationRemovedEvent(string locationKey, Location removedLocation, bool wasSuccessful, string failureReason = "")
        {
            LocationKey = locationKey;
            RemovedLocation = removedLocation;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "LocationManager";
        }
    }

    /// <summary>
    /// Event published when encounter chance is modified for a location
    /// </summary>
    public class EncounterChanceChangedEvent : GameEvent
    {
        public string LocationKey { get; set; }
        public double OldChance { get; set; }
        public double NewChance { get; set; }
        public string ChangedBy { get; set; }

        public EncounterChanceChangedEvent(string locationKey, double oldChance, double newChance, string changedBy = "")
        {
            LocationKey = locationKey;
            OldChance = oldChance;
            NewChance = newChance;
            ChangedBy = changedBy;
            Source = "LocationManager";
        }
    }
} 