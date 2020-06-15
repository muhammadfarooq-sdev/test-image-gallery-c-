import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import * as S3 from 'aws-sdk/clients/s3';

@Component({
  selector: 'file-upload',
  templateUrl: './file-upload.component.html'
})
export class FileUploadComponent {
  public forecasts: WeatherForecast[];

  @ViewChild('fileControl') fileControl;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    http.get<WeatherForecast[]>(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
      this.forecasts = result;
    }, error => console.error(error));
  }

  async uploadImage() {
    const file = this.fileControl.nativeElement.files[0];

     const contentType = file.type;
    const s3 = new S3();
    //      {
    //          accessKeyId: 'YOUR-ACCESS-KEY-ID',
    //          secretAccessKey: 'YOUR-SECRET-ACCESS-KEY',
    //          region: 'YOUR-REGION'
    //      }
    //  );
      const params = {
        Bucket: 'test-image-gallery',
        Key: `uploaded-images/${file.name}`,
          Body: file,
          ACL: 'public-read',
          ContentType: contentType
    };
    try {
      const uploadResult = await s3.upload(params).promise();
      console.log('Successfully uploaded file.', uploadResult);
      return true;
    } catch(err) {
      console.log('There was an error uploading your file: ', err);
      return false;
    }
    //this.http.post<WeatherForecast[]>(this.baseUrl + 'api/FileInfo/UploadFile').subscribe(result => {
    //  this.forecasts = result;
    //}, error => console.error(error));
  }
}

interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
