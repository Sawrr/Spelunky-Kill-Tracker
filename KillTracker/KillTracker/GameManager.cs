using System;

namespace KillTracker
{
    class GameManager
    {
        private Tracker tracker;
        private MemoryReader memoryReader;

        private int kills = 0;

        public GameManager(Tracker tracker, MemoryReader memoryReader)
        {
            this.tracker = tracker;
            this.memoryReader = memoryReader;
        }

        public void update()
        {
            // Read screen state first
            int screenState = memoryReader.ReadScreenState();
            if (screenState == 0)
            {
                // Check kill count if player is playing
                int newKills = memoryReader.ReadKills();
                if (newKills != kills)
                {
                    kills = newKills;
                    tracker.UpdateKills(kills);
                }
            }
        }
    }
}