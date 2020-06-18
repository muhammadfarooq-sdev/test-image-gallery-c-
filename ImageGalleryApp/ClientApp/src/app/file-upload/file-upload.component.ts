import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FileInfo } from '../model/fileInfo.model';
import { Form, NgForm } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent {
  public fileInfo: FileInfo;
  @ViewChild('fileUploadForm') fileUploadForm: NgForm;
  @ViewChild('fileControl') fileControl;
    fileError: string;
    successUploadMessage: string;
    errorUploadMessage: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
    private ngxSpinnerService:NgxSpinnerService) {
    this.fileInfo = {};
  }

  async uploadImage() {
    if (!(this.fileControl && this.fileControl.nativeElement)) {
      return;
    }
    this.fileError = "";
    if (!this.fileControl.nativeElement.files ||
      this.fileControl.nativeElement.files.length == 0) {
      const fileRequiredError = "File is required";
      this.fileError = fileRequiredError;
      return;
    }
    const file = this.fileControl.nativeElement.files[0];
    const sizeInKB = 500;
    const bytesInKB = 1024;
    const maxSizeInBytes = sizeInKB * bytesInKB;
    if (file.size > maxSizeInBytes) {
      const fileSizeGreaterThanMaxError = `File size greater than ${sizeInKB} KB`;
      this.fileError = fileSizeGreaterThanMaxError;
      return;
    }

    try {
      this.fileInfo.sizeInBytes = file.size;
      this.fileInfo.type = file.type;
      const formData: FormData = new FormData();
      const formDataFileKey = 'formFile';
      formData.append(formDataFileKey, file, file.name);
      for (var key in this.fileInfo) {
        formData.append(key, this.fileInfo[key]);
      }
      this.ngxSpinnerService.show();
      this.successUploadMessage = "";
      this.errorUploadMessage = "";
      const apiUrl = `${this.baseUrl}api/FileInfo/UploadFile`;
      this.http.post<FileInfo>(apiUrl, formData).subscribe(result => {
        this.ngxSpinnerService.hide();
        this.successUploadMessage = "Success";
      }, error => {
        console.error(error);
        this.errorUploadMessage = "Failed";
        this.ngxSpinnerService.hide();
      });
      return true;
    } catch (err) {
      console.error(err);
      return false;
    }
   
  }
}
