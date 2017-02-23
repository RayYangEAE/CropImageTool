Crop Image(s) Tool

I create this project because I am dealing with cropping a huge amount of image(s) in the database. Hope it also can help others.

How to use:
1. Click "..." button to select a path to store the cropped image(s);
2. Click "Select Image" button to select a single image to deal with; 
3. Click "Select Folder" button to select a folder to deal with image(s) inside;
4. Image extensions allowed: png,jpg,jpeg,jpe,jfif,tif,tiff,gif,bmp,dib,tga; but since I use WWW.LoadImageIntoTexture, only png and jpg files are cropped for now;
5. Cropped image file(s) are .png;
6. There is a green rect on the image to show the area chosen to be cropped;
7. Input Percent MinX, Percent MinY, Percent MaxX, Percent MaxY, LengthX, LengthY to adjust the chosen area;
8. If LengthX>0, LengthY>0, the rect depends on Percent MinX, Percent MinY, LengthX, LengthY; otherwise Percent MinX, Percent MinY, Percent MaxX, Percent MaxY.
