//using Akka.Persistence;
//using System.Collections.Generic;
//using System.Linq;
//using VideoSubscriptionsSaver.Messages;

//namespace VideoSubscriptionsSaver.Actors
//{
//    public class DownloadTrackerPersistentActor : ReceivePersistentActor
//    {
//        public override string PersistenceId => "db-downloads-actor"; // TODO: Read docs for this
//        private List<string> _state = new List<string>();

//        public DownloadTrackerPersistentActor()
//        {
//            Recover<string>(item => _state.Add(item));

//            Command<VideoPersistentMessages.Add>(Message => Persist(Message, m => Add(m)));
//            Command<VideoPersistentMessages.Get>(Message => Get(Message));
//            Command<VideoPersistentMessages.Delete>(Message => Delete(Message));
//        }

//        private void Add(VideoPersistentMessages.Add message)
//        {
//            _state.Add(message.File);
//        }

//        private void Get(VideoPersistentMessages.Get message)
//        {
//            Sender.Tell(_state.Last(), Self);
//        }

//        private void Delete(VideoPersistentMessages.Delete message)
//        {

//        }
//    }
//}
