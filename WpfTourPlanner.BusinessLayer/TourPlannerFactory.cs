namespace WpfTourPlanner.BusinessLayer
{
    public static class TourPlannerFactory
    {
        private static ITourPlannerManager _manager;

        public static ITourPlannerManager GetTourPlannerManager()
        {
            if (_manager == null)
            {
                _manager = new TourPlannerManagerImpl();
            }

            return _manager;
        }
    }
}