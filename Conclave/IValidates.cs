using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave {

	/// <summary>
	/// An interface for a class that provides checks
	/// to determine is its data is valid.
	/// </summary>
	/// <remarks>
	/// This was originally part of `IData`, but as
	/// that model moved toward an immutable one,
	/// the emphasis of validation was lessened
	/// and moved to be the responsibility of the
	/// model constructor. Given an immutable model
	/// it simply shoulnd't be instanciated in an
	/// invalid state.
	/// </remarks>
	public interface IValidates {

		/// <summary>
		/// Indicates whether or not a model is
		/// in a valid state.
		/// </summary>
		/// <value>
		/// `true` is the model is in a valid state;
		/// otherwise, `false`.
		/// </value>
		bool IsValid { get; }
	}
}
