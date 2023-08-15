using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedBaseAndWrapperList
{
    public abstract class WrapperListBase<TModel, TWrapper> where TModel : IModel where TWrapper : IWrapper
    {
        #region list
        // underlying list
        protected readonly ObservableCollection<TWrapper> _list = new ObservableCollection<TWrapper>();
        // Exposed IEnumerable of the list
        public IEnumerable<TWrapper> List => _list;
        #endregion

        #region actions
        // action used to rebuild lists, in an action as to not need to have field for base list
        private readonly Action _rebuildCollectionAction;
        // func that calls index of on base list
        private readonly Func<TModel, int> _indexOfFunc;
        #endregion

        #region methods
        public WrapperListBase(BaseList<TModel, TWrapper> baseList)
        {
            // creates rebuild action
            _rebuildCollectionAction = new Action(() =>
            {
                _list.Clear();
                foreach (TModel t in baseList)
                    _list.Add((TWrapper)t.ToWrapper);
            });

            // creates index of func
            _indexOfFunc = new Func<TModel, int>(model =>
            {
                return baseList.IndexOf(model);
            });

            _rebuildCollectionAction();

            // subscribes to events
            baseList.CollectionShouldRebuild += OnCollectionShouldRebuild;
            baseList.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Indexed getter 
        /// </summary>
        /// <param name="index"> Index to get wrapper item from </param>
        /// <returns> Returns item at given index </returns>
        public TWrapper this[int index] { get => _list[index]; }

        /// <summary>
        /// returns wrapper item in the list, based on reference of model item
        /// </summary>
        /// <param name="model"> item to find wrapper of </param>
        /// <returns> returns the models corrosponding wrapper </returns>
        public TWrapper? Find(TModel model)
        {
            int index = _indexOfFunc(model);
            if (index == -1)
                return default;
            return this[index];
        }

        /// <summary>
        /// Invokes list should rebuild
        /// </summary>
        protected virtual void OnCollectionShouldRebuild()
        {
            _rebuildCollectionAction();
        }

        /// <summary>
        /// Changes list based on given action, to match base list
        /// </summary>
        /// <param name="action"> Action to run on list </param>
        protected virtual void OnCollectionChanged(Action<IList> action)
        {
            action(_list);
        }
        #endregion
    }
}
