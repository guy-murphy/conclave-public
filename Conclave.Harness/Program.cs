using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Diagnostics;
using Autofac;
using Conclave.Algorithms;
using Conclave.Data.Store;
using Conclave.Extensions;
using Conclave.Map.Store;
using Conclave.Map.Store.MySql;
using Conclave.Process.User;
using Conclave.Web.CMS.Behaviour.Topicmap;
using RazorEngine;
using RazorEngine.Templating;
using Newtonsoft.Json;

using Conclave.Collections;
using Conclave.Process;
using Conclave.Data.Resolver;
using System.Text.RegularExpressions;
using Conclave.Map.Model;
using Spring.Objects.Factory.Parsing;
using MySqlTopicStore = Conclave.Map.Store.MySql._MySqlTopicStore;

namespace Conclave.Harness {
	class Program {

		private static void _importOldTopicmap() {
			using (MySqlTopicStore foreign = new MySqlTopicStore(@"Server=localhost;Database=acumen_london;Uid=root;Pwd=redacted;"))
			using (_MySqlTopicStore local = new MySqlTopicStore(@"Server=localhost;Database=conclave;Uid=root;Pwd=redacted;")) {
				foreign.Start();
				local.Start();

				// import topics
				local.Exec("delete from topic");
				using (IDataReader reader = foreign.Read(@"select * from topic")) {
					string id;
					while (reader.Read()) {
						id = reader.ReadString("act_code");
						if (!local.TopicExists(id)) {
							local.Exec("insert into topic (id) values (?id)", local.CreateParamater("?id", id));
						}
					}
				}
				// import occurrences
				local.Exec("delete from occurrence");
				using (IDataReader reader = foreign.Read(@"select * from occurence")) {
					string parent, scope, role, behaviour, reference;
					while (reader.Read()) {
						parent = reader.ReadString("aco_actref");
						scope = String.Concat(reader.ReadString("aco_lang"), "_", reader.ReadString("aco_scope"));
						reference = reader.ReadString("aco_reference");
						behaviour = reader.ReadString("aco_behaviour");
						role = reader.ReadString("aco_role");
						local.Exec("insert into occurrence (parent, scope, role, behaviour, reference) values (?parent, ?scope, ?role, ?behaviour, ?reference)",
							local.CreateParamater("?parent", parent),
							local.CreateParamater("?scope", scope),
							local.CreateParamater("?role", role),
							local.CreateParamater("?behaviour", behaviour),
							local.CreateParamater("?reference", reference)
						);
					}
				}
				// import assocs
				local.Exec("delete from association");
				using (IDataReader reader = foreign.Read(@"select * from association")) {
					string id, parent, reference, type, role, scope;
					while (reader.Read()) {
						id = reader.ReadString("aca_code");
						parent = reader.ReadString("aca_source");
						reference = reader.ReadString("aca_reference");
						type = reader.ReadString("aca_type");
						role = reader.ReadString("aca_role");
						scope = reader.ReadString("aca_scope");

						local.Exec(@"
insert into association 
	(id, parent, type, scope, role, reference) values (?id, ?parent, ?type, ?scope, ?role, ?reference)
on duplicate key
	update reference=?reference
",
							local.CreateParamater("?id", id),
							local.CreateParamater("?parent", parent),
							local.CreateParamater("?type", type),
							local.CreateParamater("?scope", scope),
							local.CreateParamater("?role", role),
							local.CreateParamater("?reference", reference)
						);
					}
				}
				// import metadata
				local.Exec("delete from metadata");
				using (IDataReader reader = foreign.Read(@"select * from metadata")) {
					string parent, scope, name, value;
					while (reader.Read()) {
						parent = reader.ReadString("acm_ref");
						scope = String.Concat(reader.ReadString("acm_lang"), "_", reader.ReadString("acm_scope"));
						name = reader.ReadString("acm_name");
						value = reader.ReadString("acm_value");
						local.Exec("insert into metadata (parent, scope, name, value) values (?parent, ?scope, ?name, ?value)",
							local.CreateParamater("?parent", parent),
							local.CreateParamater("?scope", scope),
							local.CreateParamater("?name", name),
							local.CreateParamater("?value", value)
						);
					}
				}

			}
		}

		private static string _markdown = @"
Lorem ipsum dolor sit amet, id inimicus adipiscing per, an sea mazim liber dolores, ad appellantur complectitur mel. Has suas fastidii dignissim id. Habemus prodesset ex mea. Eam no zril aperiri deleniti, et usu rebum graeci scripta.

An qui tale duis, ne mei legere appareat. Pri aliquip albucius convenire ne, usu te harum perfecto. Cu has nibh melius, cum enim decore cu. Sumo iriure perpetua mei ex. Eos option audire fuisset te, purto voluptatum mei te, at adhuc facilisis has.

Ne posse tantas mei, inani ludus necessitatibus cu est, ei detraxit tractatos sed. Sed et appareat principes interpretaris. Et duo vitae probatus. Has porro inermis delicata id, pri nullam pericula et, quot consulatu cotidieque te his. Usu ad brute laboramus, ei eum lorem tollit nostrud. Nam cu mentitum explicari, malis iriure reprimique pri an, ius nonumy utroque eu.

Quo ut solet appareat atomorum, eu fuisset fabellas pri, sea ad nominavi torquatos. Dolor vidisse vocibus ut pro, mea omnes utinam an. Corpora moderatius cum an. Cum no fuisset conceptam argumentum, tale tamquam posidonium ne his. Ut vel quando scribentur, cum in lorem nobis iudicabit.

Cu mea minim vocibus, in nam dissentias consectetuer. Probo solum eam ei, ea oporteat accommodare ius. Ius inermis scaevola ea. Minim simul alienum an his. Has quis putent qualisque eu, simul efficiendi appellantur et mel, ne debitis vulputate qui.
";

		private static void EditOccurrenceWiki() {
			using (_MySqlTopicStore store = new MySqlTopicStore(@"Server=localhost;Database=conclave;Uid=root;Pwd=redacted;")) {
				store.Start();



				DateTime start = DateTime.Now;
				Topic t1 = store.GetTopic("florida");

				Occurrence o1 = t1.Occurrences.FirstOrDefault(o => o.Reference == "self" && o.Behaviour == "markdown");
				Console.WriteLine(t1.ToXml());

				if (o1 != null) {
					Occurrence o2 = o1.Mutate(m =>
					{
						m.StringData = _markdown;
						return m;
					});
					Console.WriteLine(o2.ToXml());
					store.AddOccurrence(o2);
					Occurrence o3 = store.GetOccurrence(o2.Parent, o2.Scope, o2.Role, o2.Behaviour);
					Console.WriteLine(o3.ToXml());
				}

				//store.AddTopic(t1);
				//Topic t2 = store.GetTopic("florida");
				//Console.WriteLine(t2.ToXml());

				TimeSpan duration = DateTime.Now - start;
				Console.WriteLine("{0}ms", duration.Milliseconds);

			}
		}
		
		private static void Main(string[] args) {
			Console.WriteLine("Press [ENTER] to run harness code...");
			Console.ReadLine();
			Console.WriteLine("==================================================");

			Dictionary<string, string> values = new Dictionary<string, string> {
				{"name", "Guy Murphy"},
				{"age", "44"}
			};
			Console.WriteLine("Name: |name| Age: |age| and stuffs.".ReplaceKeys(values));

			Console.WriteLine("==================================================");
			Console.WriteLine("\nPress [ENTER] to close the harness console.");
			Console.ReadLine();
		}

		private static string _pop = @"
	declare @msg_id int
	declare @msg_ref varchar(50)
	declare @msg_data varchar(1024)

	begin tran TRAN_1

		select top 1 
			@msg_id = id,
			@msg_ref = reference,
			@msg_data = data
		from msgq with(readpast, updlock) where lock=0 order by id asc

		print 'processing msg_id #' + cast(@msg_id as varchar)
		update msgq set lock=1 where id = @msg_id

		select 
			@msg_id as id,
			@msg_ref as reference,
			@msg_data as data

	commit
";

		public enum MsgControl {
			ReadMessage
		}

		public static void Dataflow() {
			string connectionString = @"Server=.\CONCLAVE;Database=conclave;User Id=user;Password=password;";

			BufferBlock<MsgControl> control = new BufferBlock<MsgControl>();
			BroadcastBlock<Tuple<int, string, string>> bus = new BroadcastBlock<Tuple<int, string, string>>(tuple => tuple);

			TransformBlock<MsgControl, Tuple<int, string, string>> readBlock = new TransformBlock<MsgControl, Tuple<int, string, string>>(
				_ => {
					int id = 0;
					string reference = String.Empty;
					string data = String.Empty;
					using (SqlServerTopicStore store = new SqlServerTopicStore(connectionString)) {
						store.Start();
						using (IDataReader reader = store.Read(_pop)) {
							while (reader.Read()) {
								id = reader.ReadInt("id");
								reference = reader.ReadString("reference");
								data = reader.ReadString("data");
							}
						}
					}
					return new Tuple<int, string, string>(id, reference, data);
				}
			);

			

			ActionBlock<Tuple<int,string,string>> printSink = new ActionBlock<Tuple<int, string, string>>(tuple => Console.WriteLine("#{0}: {1} => {2} ON {3}", tuple.Item1, tuple.Item2, tuple.Item3, Thread.CurrentThread.ManagedThreadId));
			ActionBlock<Tuple<int, string, string>> deleteSink = new ActionBlock<Tuple<int, string, string>>(tuple => Console.WriteLine("DELETE #{0} ON {1}", tuple.Item1, Thread.CurrentThread.ManagedThreadId));

			control.LinkTo(readBlock, msgControl => msgControl == MsgControl.ReadMessage);
			control.LinkTo(DataflowBlock.NullTarget<MsgControl>()); // messages must be handled, prevents a deadlock

			readBlock.LinkTo(bus);
			bus.LinkTo(printSink);
			bus.LinkTo(deleteSink);

			Timer timer = new Timer(_ => control.Post(MsgControl.ReadMessage), null, 0, 1);

			//for (int i = 0; i < 100; i++) {
			//	control.Post(MsgControl.ReadMessage);
			//}
		}

		public static void TestRead() {
			using (SqlServerTopicStore store = new SqlServerTopicStore(@"Server=.\CONCLAVE;Database=conclave;User Id=user;Password=password;")) {
				store.Start();
				Stopwatch watch = new Stopwatch();
				watch.Start();
				for (int i = 0; i < 1000; i++) {
					using (IDataReader reader = store.Read(_pop)) {
						while (reader.Read()) {
							Console.WriteLine("#{0}: {1}", reader.ReadInt("id"), reader.ReadString("reference"));
						}
					}
				}
				watch.Stop();
				Console.WriteLine("time: {0}ms", watch.ElapsedMilliseconds);
				Console.WriteLine("average: {0}ms", watch.ElapsedMilliseconds / 1000);

			}
		}

		public static void LoadData() {
			using (SqlServerTopicStore store = new SqlServerTopicStore(@"Server=.\CONCLAVE;Database=conclave;User Id=user;Password=password;" )) {
				store.Start();
				using (StreamReader reader = File.OpenText(@"E:\Data\Unabr.dict")) {
					string line;
					int count = 0;
					while ((line = reader.ReadLine()) != null) {
						store.Exec(String.Format(@"insert into msgq (reference, data) values (@word, @word)"), store.CreateParamater("@word", line));
						count++;
						if (count % 100 == 0) Console.WriteLine(count);
					}
				}
			}
		}

		
	}
}
