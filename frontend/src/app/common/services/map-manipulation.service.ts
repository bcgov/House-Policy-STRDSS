import { Injectable } from '@angular/core';
import { Subject, Observable  } from 'rxjs';
import { LatLonCoordinate } from '../models/lat-lon-coordinates.model';
import { StrApplication } from '../models/application.model';
import { Coordinate } from 'ol/coordinate';

@Injectable({
  providedIn: 'root'
})
export class MapManipulationService {
  coordinateViewRequested = new Subject<LatLonCoordinate>();
  applicationPin = new Subject<StrApplication>();

  private cleanUpVectorLayersSubject = new Subject<void>();
  cleanUpVectorLayers$: Observable<void> = this.cleanUpVectorLayersSubject.asObservable();

  private circleFilterSubject = new Subject<{ center: Coordinate, radius: number }>();
  circleFilter$: Observable<{ center: Coordinate, radius: number }> = this.circleFilterSubject.asObservable();

  constructor() { }

  triggerCleanUpVectorLayers(): void {
    this.cleanUpVectorLayersSubject.next();
  }

  triggerCircleFilter(circle: { center: Coordinate, radius: number }): void {
    this.circleFilterSubject.next(circle);
  }
}
