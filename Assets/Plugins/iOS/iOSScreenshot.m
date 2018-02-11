//
//  iOSScreenshot.m
//
//
//  Created by Ryan on 20/03/2013.
//
//

int saveToGallery(const char *path)
{
    NSString *imagePath = [NSString stringWithUTF8String:path];
    
    NSLog(@"###### This is the file path being passed: %@", imagePath);
    
    if(![[NSFileManager defaultManager] fileExistsAtPath:imagePath])
    {
        NSLog(@"###### Early exit - file doesn't exist");
        return 0;
    }
    
    UIImage *image = [UIImage imageWithContentsOfFile:imagePath];
    
    if(image)
    {
        NSLog(@"###### Trying to write image");
        UIImageWriteToSavedPhotosAlbum( image, nil, NULL, NULL );
        return 1;
    }
    
    return 0;
}
