using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using photo.exif;

namespace GpsUnitTest
{
    [TestClass]
    public class GpsTest
    {
        [TestMethod]
        public void TestImage()
        {
            FileInfo fi = new FileInfo(Path.Combine(Environment.CurrentDirectory,
                @"..\..\..\..\..\Images\Samsung SIII.jpg")); // contains GPS coordinates

            Parser par = new Parser();

            // id 1 through 4 have GPS data
            IEnumerable<ExifItem> gpsItems = par.Parse(fi.FullName).Where(x => 1 <= x.Id && x.Id <= 4).ToList();

            Debug.Assert(gpsItems.Any());

            foreach ( ExifItem x in gpsItems )
            {
                Debug.WriteLine(x);
                Debug.WriteLine("");
            }

            // latitude and longitude arrays
            IEnumerable<ExifItem> arrayItems = gpsItems.Where(x => x.Id == 2 || x.Id == 4);
            foreach ( ExifItem x in arrayItems )
            {
                Debug.Assert(x.Value is URational[]);
            }
        }
    }
}
