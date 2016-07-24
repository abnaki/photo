Photo Exif  
===============

Abnaki fork of https://github.com/fraxedas/photo

Improved Converter.cs.   When an array of rationals is required, as in the EXIF standard for GPS Latitude or Longitude, ExifItem.Value will be an array URational[].  In the original code the Value was only the most signficant URational.
