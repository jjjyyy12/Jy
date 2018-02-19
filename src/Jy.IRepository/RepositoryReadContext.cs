using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.IRepositories
{
    public abstract class RepositoryReadContext<TSession> : IRepositoryReadContext<TSession>
        where TSession : class
    {
        #region Private Fields
        private readonly TSession session;
        private readonly Guid id = Guid.NewGuid();
        #endregion

        #region Ctor
        protected RepositoryReadContext(TSession session)
        {
            this.session = session;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the unique identifier of the current repository context.
        /// </summary>
        public Guid Id => this.id;

        /// <summary>
        /// Gets the instance of a session object.
        /// </summary>
        /// <remarks>
        /// A session object usually maintains the connection between the repository and
        /// its backend infrastructure. For example, it can be the DbContext in Entity Framework
        /// repository implementation, or an ISession instance in NHibernate implementation.
        /// </remarks>
        public TSession Session => this.session;

        /// <summary>
        /// Gets the instance of a session object.
        /// </summary>
        /// <remarks>
        /// A session object usually maintains the connection between the repository and
        /// its backend infrastructure. For example, it can be the DbContext in Entity Framework
        /// repository implementation, or an ISession instance in NHibernate implementation.
        /// </remarks>
        object IRepositoryReadContext.Session => this.session;

        #endregion

        #region Public Methods

        public virtual void Dispose(){ }
        #endregion

        #region Protected Methods

        #endregion

    }
}
