import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  Input,
  OnInit,
} from '@angular/core';
import Map from 'ol/Map.js';
import OSM from 'ol/source/OSM.js';
import TileLayer from 'ol/layer/Tile.js';
import View from 'ol/View.js';
import * as proj from 'ol/proj';
import { MapManipulationService } from '../../services/map-manipulation.service';
import { LatLonCoordinate } from '../../models/lat-lon-coordinates.model';

import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import { Point, LineString } from 'ol/geom';
import { Style, Icon } from 'ol/style';
import Feature from 'ol/Feature';
import { Draw, Modify, Snap } from 'ol/interaction';
import { StrApplication } from '../../models/application.model';

const DEFAULT_ZOOM_LEVEL = 17;
@Component({
  selector: 'app-map',
  standalone: true,
  imports: [],
  templateUrl: './map.component.html',
  styleUrl: './map.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MapComponent implements OnInit {
  @Input() map!: Map;

  private vectorLayer!: VectorLayer<VectorSource<Feature>>;
  private vectorSource!: VectorSource<Feature>;

  constructor(
    private elementRef: ElementRef,
    private mapService: MapManipulationService
  ) {}

  ngOnInit() {
    this.initializeMap();
    this.mapService.applicationPin.subscribe((app) => {
      this.addAddressPin(app, DEFAULT_ZOOM_LEVEL);
    });
    this.mapService.coordinateViewRequested.subscribe((coordinates) => {
      this.showAddress(coordinates, DEFAULT_ZOOM_LEVEL);
    });
    this.mapService.cleanUpVectorLayers$.subscribe(() => {
      this.cleanUpVectorLayers();
    });
  }

  initializeMap(): void {
    // Initialize VectorSource and VectorLayer
    this.vectorSource = new VectorSource();
    this.vectorLayer = new VectorLayer({
      source: this.vectorSource,
    });

    const circleSource = new VectorSource();
    const circleLayer = new VectorLayer({
      source: circleSource,
    });

    const draw = new Draw({
      source: circleSource,
      type: 'Circle',
    });

    draw.on('drawend', (event: any) => {
      circleSource.clear();
      const circleFeature = event.feature;
      const circleGeometry = circleFeature.getGeometry();
      const center = circleGeometry.getCenter();
      const radius = circleGeometry.getRadius();

      this.mapService.triggerCircleFilter({ center, radius });
      console.log(`center: ${center}, radius: ${radius}`);
    });

    this.map = new Map({
      view: new View({
        center: [-13732502.177927643, 6178193.614991034],
        zoom: DEFAULT_ZOOM_LEVEL,
      }),
      layers: [
        new TileLayer({
          source: new OSM(),
        }),
        this.vectorLayer,
        circleLayer,
      ],
      target: 'ol-map',
    });

    this.map.addInteraction(draw);

    // const modify = new Modify({ source: this.vectorSource });
    // this.map.addInteraction(modify);

    // const snap = new Snap({ source: this.vectorSource });
    // this.map.addInteraction(snap);
  }

  showAddress(latLonCoordinate: LatLonCoordinate, zoom: number): void {
    const coordinate = proj.fromLonLat(
      [latLonCoordinate.lat, latLonCoordinate.lon],
      'EPSG:3857'
    );

    this.map.setView(
      new View({
        center: coordinate,
        zoom: zoom,
      })
    );
    this.map.render();
  }

  /**
   * Add a pin to the map representing a specific address with a given status.
   *
   * @param {StrApplication} app - The details of the pin, including latitude, longitude, and status.
   * @param {number} zoom - The zoom level for the map.
   * @returns {void}
   */
  addAddressPin(app: StrApplication, zoom: number): void {
    // Convert the latitude and longitude coordinates to the map projection
    const coordinate = proj.fromLonLat(
      [app.latitude, app.longitude],
      'EPSG:3857'
    );

    // Convert the status to lowercase for consistency
    const appStatus = app.complianceStatus.codeValue.toLowerCase();

    // Dynamically create an Icon style with a color based on the appStatus
    const pinColor = this.getPinColor(appStatus);
    const pinStyle = new Style({
      image: new Icon({
        src: '/assets/white-pin.png', // Path to the pin image
        color: pinColor,
        scale: 0.06, // Scale of the image, consistent across zoom
      }),
    });

    const feature = new Feature(new Point(coordinate));
    feature.setStyle(pinStyle);

    this.vectorSource.addFeature(feature);
    this.adjustMapExtent();
  }

  /**
   * Adjusts the map view to fit the extent of all features in the VectorSource.
   * If VectorSource is not initialized, a warning is logged.
   *
   * @returns {void}
   */
  adjustMapExtent(): void {
    // Check if VectorSource is initialized
    if (this.map && this.vectorSource) {
      setTimeout(() => {
        // Get the extent of all features in the VectorSource
        const extent = this.vectorSource.getExtent();

        // Fit the map view to the extent of all features
        this.map.getView().fit(extent, {
          padding: [20, 20, 20, 20], // Adjust padding as needed
          duration: 100, // Animation duration in milliseconds, adjust as needed
        });

        // Render the map
        this.map.render();
      }, 0);
    } else {
      // Log a warning if VectorSource is not initialized
      console.warn('VectorSource not initialized. Adjusting view skipped.');
    }
  }

  /**
   * Get the pin color based on the status.
   *
   * @param {string} status - The status of the application
   * @returns {string} - The color code for the pin.
   */
  getPinColor(status: string) {
    // Add logic here to determine the color based on the status
    switch (status) {
      case 'pending':
        return 'yellow';
      case 'compliant':
        return 'green';
      case 'non-compliant':
        return 'red';
      default:
        return 'white'; // Default color
    }
  }

  /**
   * Cleans up Vector Layers from the map by removing all existing Vector Layers.
   *
   * @private
   * @returns {void}
   */
  private cleanUpVectorLayers(): void {
    // Iterate through all layers of the map
    const layers = this.map.getLayers().getArray();
    layers.forEach((layer) => {
      // Check if the layer is an instance of VectorLayer
      if (layer instanceof VectorLayer) {
        // Get the source associated with the VectorLayer
        const vectorSource = layer.getSource();

        // Clear features from the source instead of removing the layer
        vectorSource.clear();
      }
    });
  }
}
