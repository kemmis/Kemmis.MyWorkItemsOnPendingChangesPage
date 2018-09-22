using System;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Common
{
    public class SortedObservableRangeCollection<T>
        : ObservableRangeCollection<T> where T : IComparable<T>
    {
        private const string _InsertErrorMessage
            = "Inserting and moving an item using an explicit index are not support by sorted observable collection";

        protected override void InsertItem(int index, T item)
        {
            if (Count == 0)
            {
                base.InsertItem(0, item);
                return;
            }

            index = Compare(item, 0, Count - 1);

            base.InsertItem(index, item);
        }

        private int Compare(T item, int lowIndex, int highIndex)
        {
            var compareIndex = (lowIndex + highIndex) / 2;

            if (compareIndex == 0) return SearchIndexByIteration(lowIndex, highIndex, item);

            var result = item.CompareTo(this[compareIndex]);

            if (result < 0)
            {
                //item precedes indexed obj in the sort order

                if (lowIndex + compareIndex < 100 || compareIndex == (lowIndex + compareIndex) / 2)
                    return SearchIndexByIteration(lowIndex, compareIndex, item);

                return Compare(item, lowIndex, compareIndex);
            }

            if (result > 0)
            {
                //item follows indexed obj in the sort order

                if (compareIndex + highIndex < 100 || compareIndex == (compareIndex + highIndex) / 2)
                    return SearchIndexByIteration(compareIndex, highIndex, item);

                return Compare(item, compareIndex, highIndex);
            }

            return compareIndex;
        }

        /// <summary>
        ///     Iterates through sequence of the collection from low to high index
        ///     and returns the index where to insert the new item
        /// </summary>
        private int SearchIndexByIteration(int lowIndex, int highIndex, T item)
        {
            for (var i = lowIndex; i <= highIndex; i++)
                if (item.CompareTo(this[i]) < 0)
                    return i;
            return Count;
        }

        /// <summary>
        ///     Adds the item to collection by ignoring the index
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            InsertItem(index, item);
        }

        /// <summary>
        ///     Throws an error because inserting an item using an explicit index
        ///     is not support by sorted observable collection
        /// </summary>
        [Obsolete(_InsertErrorMessage)]
        public new void Insert(int index, T item)
        {
            throw new NotSupportedException(_InsertErrorMessage);
        }

        /// <summary>
        ///     Throws an error because moving an item using explicit indexes
        ///     is not support by sorted observable collection
        /// </summary>
        [Obsolete(_InsertErrorMessage)]
        public new void Move(int oldIndex, int newIndex)
        {
            throw new NotSupportedException(_InsertErrorMessage);
        }
    }
}