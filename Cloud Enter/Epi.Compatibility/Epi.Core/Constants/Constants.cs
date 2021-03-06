using System;

namespace Epi
{
    /// <summary>
    /// General purpose constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Code value for Yes
        /// </summary>
        public const int YES = 1;

        /// <summary>
        /// Code value for No
        /// </summary>
        public const int NO = 0;

        /// <summary>
        ///  Code value for normal
        /// </summary>
        public const int NORMAL = 1;

        /// <summary>
        /// Code value for deleted records.
        /// </summary>
        public const int DELETED = 0;

        /// <summary>
        /// Used to go from the grid setting to pixels.
        /// </summary>
        public const int GRID_FACTOR = 3;

        /// <summary>
        /// Used to separate listed items in the metaFields table.
        /// </summary>
        public const Char LIST_SEPARATOR = ',';

        public const string VARIABLE_NAME_TEST_TOKEN = "zydpbwza"; //"VARNAMETESTTOKEN";
    }
}