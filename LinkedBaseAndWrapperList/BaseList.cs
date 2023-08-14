using System.Collections;

namespace LinkedBaseAndWrapperList
{
    /// <summary>
    /// Base list, the list containing the data models, and the list wich should be interacted with
    /// Can have many wrapper lists
    /// </summary>
    /// <typeparam name="TModel"> Type of the model, should implement IModel</typeparam>
    /// <typeparam name="TWrapper"> Type of the wrapper, should implement IWrapper </typeparam>
    public class BaseList<TModel, TWrapper> where TModel : IModel where TWrapper : IWrapper
    {
        #region list
        // Underlying list
        private readonly List<TModel> _list = new List<TModel>();
        // Public accesor for the list, but as IEnumerable
        public IEnumerable<TModel> List => _list;
        #endregion

        #region events
        // Event containing action, that moves, removes or adds items, such that the state of the wrapper list matches the state of the base list
        public event Action<Action<IList>>? CollectionChanged;
        // Event that notifies the wrapper list, and makes it rebuild its list
        public event Action? CollectionShouldRebuild;
        #endregion

        #region methods to interact with list
        /// <summary>
        /// Index operator? Gets or sets value at index, sets value in base and wrapper lists
        /// </summary>
        /// <param name="index"> Index of which to set and get </param>
        /// <returns> Returns value at index </returns>
        public TModel this[int index] { get => _list[index]; set => Do((list, item) => list[index] = item[0], value); }

        /// <summary>
        /// Adds a collection to base and wrapper lists
        /// </summary>
        /// <param name="collection"> Collection to add </param>
        public void AddRange(IEnumerable<TModel> collection)
        {
            Do((list, items) =>
            {
                foreach (var item in items)
                    list.Add(item);
            }
            , collection.ToArray());
        }

        /// <summary>
        /// Adds an item to base and wrapper lists
        /// </summary>
        /// <param name="item"> Item to add </param>
        public void Add(TModel item)
        {
            Do((list, item) => list.Add(item[0]), item);
        }

        /// <summary>
        /// Gets index of item
        /// </summary>
        /// <param name="item"> Item to get index of </param>
        /// <returns> Returns the index if found, otherwise -1 </returns>
        public int IndexOf(TModel item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// clears base and wrapper lists
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            CollectionShouldRebuild?.Invoke();
        }

        /// <summary>
        /// Swaps items of index a and b
        /// </summary>
        /// <param name="a"> Index of first item </param>
        /// <param name="b"> Index of second item </param>
        public void Swap(int a, int b)
        {
            Do(list =>
            {
                var temp = list[a];
                list[a] = list[b];
                list[b] = temp;
            });
        }
        #endregion

        #region private methods
        /// <summary>
        /// Helper method to cut down on code duplication
        /// Runs the action on the base list, and invokes ListChanged with the same action, so that the same change is made to the wrapper list
        /// </summary>
        /// <param name="action"> The action which is to be performed on base and wrapper lists </param>
        private void Do(Action<IList> action)
        {
            action.Invoke(_list);
            CollectionChanged?.Invoke(action);
        }

        /// <summary>
        /// Same function as the other Do function, but now new values are added or exchanged, and therefore a conversion needs to be made,
        /// from TModel to TWrapper
        /// </summary>
        /// <param name="action"> The action which is to be performed on base and wrapper lists, with the given item or items </param>
        /// <param name="items"> Item or items, needed in the action </param>
        private void Do(Action<IList, IList> action, params TModel[] items)
        {
            action.Invoke(_list, items);


            List<TWrapper> convertedValues = new List<TWrapper>();
            foreach (var item in items)
                convertedValues.Add((TWrapper)item.ToWrapper);

            // creates new Action<IList> by invoking action and passing in the converted values
            CollectionChanged?.Invoke(list => action.Invoke(list, convertedValues));
        }
        #endregion
    }
}
