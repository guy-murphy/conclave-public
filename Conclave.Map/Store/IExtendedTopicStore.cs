using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Map.Store {
	public interface IExtendedTopicStore: ITopicStore {

		TopicStoreExtendedFeatures Features { get; }
		bool HasFeature(TopicStoreExtendedFeatures feature);



	}
}
