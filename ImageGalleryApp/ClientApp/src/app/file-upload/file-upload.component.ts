import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FileInfo } from '../model/fileInfo.model';

@Component({
  selector: 'file-upload',
  templateUrl: './file-upload.component.html'
})
export class FileUploadComponent {
  public fileInfo: FileInfo;

  @ViewChild('fileControl') fileControl;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.fileInfo = {};
  }

  async uploadImage() {
    if (!(this.fileControl && this.fileControl.nativeElement &&
      this.fileControl.nativeElement.files &&
      this.fileControl.nativeElement.files.length > 0)) {
      return;
    }
    const file = this.fileControl.nativeElement.files[0];
    
    try {
      this.fileInfo.sizeInBytes = file.size;
      this.fileInfo.type = file.type;
      const formData: FormData = new FormData();
      const formDataFileKey = 'formFile';
      formData.append(formDataFileKey, file, file.name);
      for (var key in this.fileInfo) {
        formData.append(key, this.fileInfo[key]);
      }
      this.http.post<FileInfo>(this.baseUrl + 'api/FileInfo/UploadFile', formData).subscribe(result => {
        this.fileInfo = result;
    }, error => console.error(error));
      return true;
    } catch(err) {
      console.log('There was an error uploading your file: ', err);
      return false;
    }
   
  }
}
