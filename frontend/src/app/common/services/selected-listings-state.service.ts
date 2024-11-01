import { Injectable } from '@angular/core';
import { ListingDetails } from '../models/listing-details';

@Injectable({
  providedIn: 'root'
})
export class SelectedListingsStateService {

  selectedListings!: Array<ListingDetails>;

  constructor() { }
}
