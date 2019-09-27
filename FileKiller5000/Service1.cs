using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace FileKiller5000
{
    public partial class FileKillerService : ServiceBase
    {
        #region Constants

        private string _baseKey = "Software\\JVM";

        private string _path1Key = "Path1";

        private string _path2Key = "Path2";

        private string _path3Key = "Path3";

        private string _yearKey = "Year";

        private string _monthKey = "Month";

        private string _dayKey = "Day";

        private object _lockObject = new object();

        #endregion

        #region Fields

        /// <summary>
        /// Registry key nesnesi
        /// </summary>
        private RegistryKey _regKey;

        /// <summary>
        /// Thread sonlandırma nesnesi
        /// </summary>
        private CancellationTokenSource _cancelToken;

        #endregion

        #region Methods

        /// <summary>
        /// Registry içerisine kayıt ekler
        /// </summary>
        /// <param name="key">Kayıt eklenecek registry</param>
        /// <param name="value">Kayıt değeri</param>
        private void AddValueToRegistry(RegistryKey regKey, string keyValue, string value)
        {
            if (null != regKey)
            {
                if (null == regKey.GetValue(keyValue))
                {
                    regKey.SetValue(keyValue, value);
                }
            }
        }

        /// <summary>
        /// Key değeri yok ise registry içerisine kaydını ekler
        /// </summary>
        private void AddKeyToRegistryIfNotExist()
        {
            RegistryKey localMachine;

            if (Environment.Is64BitOperatingSystem)
            {
                localMachine = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                localMachine = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
            }

            _regKey = localMachine.OpenSubKey(_baseKey, true);

            if (_regKey == null)
            {
                _regKey = localMachine.CreateSubKey(_baseKey);
            }

            AddValueToRegistry(_regKey, _path1Key, @"C:\Tandemgold\File1");
            AddValueToRegistry(_regKey, _path2Key, @"C:\Tandemgold\File2");
            AddValueToRegistry(_regKey, _path3Key, @"C:\Tandemgold\File3");
            AddValueToRegistry(_regKey, _yearKey, "2019");
            AddValueToRegistry(_regKey, _monthKey, "12");
            AddValueToRegistry(_regKey, _dayKey, "10");
        }

        /// <summary>
        /// Registry'den değeri alır
        /// </summary>
        /// <param name="key">Key değeri</param>
        /// <returns>Değer var ise değeri, yoksa boş string döndürür</returns>
        private string GetValue(string key)
        {
            string result = string.Empty;

            if (null != _regKey)
            {
                object value = _regKey.GetValue(key);

                if (null != value)
                {
                    result = value.ToString();
                }
            }

            return result;
        }
        
        /// <summary>
        /// Girilen yoldaki dosyayı siler
        /// </summary>
        /// <param name="pathKey">Dosya yolunu tutan key değeri</param>
        private void DeleteFile(string pathKey)
        {
            string filePath = GetValue(pathKey);

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                catch(IOException)
                {
                    //Silme işlemi başarısız
                }
            }
        }

        /// <summary>
        /// Tarih kayıtlı değerden büyükmü diye kontrol eder
        /// </summary>
        /// <returns>Tarih büyükse true, değil ise false döndürür</returns>
        private bool DateControl()
        {
            bool result = false;

            string year = GetValue(_yearKey);
            string month = GetValue(_monthKey);
            string day = GetValue(_dayKey);

            if (!string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(day))
            {
                DateTime deleteTime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));

                int comparisonResult = DateTime.Compare(DateTime.Now, deleteTime);

                if (0 <= comparisonResult)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Silme işlemini periyodik olarak gerçekleştiren thread metodu
        /// </summary>
        /// <param name="token">Thread context</param>
        private void RunService(object context)
        {
            while (true)
            {
                lock (_lockObject)
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                try
                {
                    if (DateControl())
                    {
                        DeleteFile(_path1Key);
                        Thread.Sleep(5000);

                        DeleteFile(_path2Key);
                        Thread.Sleep(5000);

                        DeleteFile(_path3Key);
                        Thread.Sleep(5000);
                    }
                }
                catch(Exception)
                { }
            }
        }


        #endregion

        #region Constructors

        public FileKillerService()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods - Overrides

        protected override void OnStart(string[] args)
        {
            AddKeyToRegistryIfNotExist();

            _cancelToken = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(RunService);
        }

        protected override void OnStop()
        {
            lock (_lockObject)
            {
                if (null != _cancelToken)
                {
                    _cancelToken.Cancel();
                }
            }
        }

        #endregion
    }
}
