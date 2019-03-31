using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ConsoleApp1
{
    /// <summary>
    ///     Defines a node that participates in a hierarchy of progress.
    /// </summary>
    public sealed class ProgressNode : IEnumerable<ProgressNode>
    {
        public ProgressNode([CanBeNull] ProgressNode parent = null)
        {
            Parent = parent;
        }

        #region IEnumerable<ProgressNode> Members

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<ProgressNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when progress has changed.
        /// </summary>
        public event EventHandler ProgressChanged;

        private void OnProgressChanged()
        {
            ProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a child of this instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProgressNode CreateChild([CanBeNull] string name = null)
        {
            var child = new ProgressNode(this) {Name = name};

            Children.Add(child);

            return child;
        }

        /// <summary>
        ///     Gets the leaf nodes of this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProgressNode> GetLeafs()
        {
            return this.Flatten(s => s).Where(s => s.IsLeaf);
        }

        /// <summary>
        ///     Gets the count of total elements for this instance and its descendants.
        /// </summary>
        /// <returns></returns>
        public int GetTotalElements()
        {
            if (IsRoot)
            {
                return this.SelectMany(s => s).Sum(s => s.Progress.Total);
            }

            if (IsNode)
            {
                return this.Sum(s => s.Progress.Total);
            }

            if (IsLeaf)
            {
                return Progress.Total;
            }

            throw new InvalidOperationException("Instance state is not valid.");
        }

        /// <summary>
        ///     Gets the count of elements processed for this instance and its descendants.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProcessed()
        {
            if (IsRoot)
            {
                return this.SelectMany(s => s).Sum(s => s.Progress.Value);
            }

            if (IsNode)
            {
                return this.Sum(s => s.Progress.Value);
            }

            if (IsLeaf)
            {
                return Progress.Value;
            }

            throw new InvalidOperationException("Instance state is not valid.");
        }

        /// <summary>
        ///     Sets the progress of this instance.
        /// </summary>
        /// <param name="total"></param>
        /// <param name="value"></param>
        public void SetProgress(int total, int value)
        {
            if (total <= 0)
                throw new ArgumentOutOfRangeException(nameof(total));

            if (value <= 0 || value > total)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (!IsLeaf)
                throw new InvalidOperationException("Progress can only be set on a leaf node.");

            Progress.Total = total;
            Progress.Value = value;

            Root.OnProgressChanged();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Progress)}: {Progress}";
        }

        #endregion

        #region Private Properties

        private IList<ProgressNode> Children { get; } = new List<ProgressNode>();

        private bool IsLeaf => Parent != null && !Children.Any();

        private bool IsNode => Parent != null && Children.Any();

        private bool IsRoot => Parent == null;

        private Progress Progress { get; } = new Progress();

        private ProgressNode Root
        {
            get
            {
                var current = this;

                while (current.Parent != null)
                {
                    current = current.Parent;
                }

                return current;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets the parent of this instance.
        /// </summary>
        public ProgressNode Parent { get; }

        #endregion
    }
}