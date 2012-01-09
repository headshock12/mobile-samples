using System;
using System.Collections.Generic;

namespace MWC.BL.Managers
{
	public static class NewsManager
	{
		static object _locker = new object();
		
		public static event EventHandler UpdateStarted = delegate {};
		public static event EventHandler UpdateFinished = delegate {};
		public static bool IsUpdating
		{
			get { return _isUpdating; }
			set { _isUpdating = value; }
		}
		private static bool _isUpdating = false;
		
		static NewsManager ()
		{}

        public static RSSEntry GetNews(int newsID)
        {
            return DAL.DataManager.GetNews(newsID);
        }

		public static IList<RSSEntry> GetNews()
		{
			return new List<RSSEntry> ( DAL.DataManager.GetNews () );
		}

		public static void Update()
		{
			// make this a critical section to ensure that access is serial
			lock(_locker)
			{
				SAL.RSSParser<RSSEntry> _newsParser = new SAL.RSSParser<RSSEntry>(Constants.NewsUrl);

				_isUpdating = true;
				_newsParser.Refresh(delegate {
					var news = _newsParser.AllItems;	
					
					DAL.DataManager.DeleteTweets ();
					DAL.DataManager.SaveNews (news);

					UpdateFinished (null, EventArgs.Empty);
					_isUpdating = false;
				});
			}
		}
	}
}

