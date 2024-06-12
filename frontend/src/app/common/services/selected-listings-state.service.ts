import { Injectable } from '@angular/core';
import { ListingSearchState } from '../models/listing-search-state';
import { ListingDetails } from '../models/listing-details';

@Injectable({
  providedIn: 'root'
})
export class SelectedListingsStateService {

  selectedListings!: Array<ListingDetails>;

  constructor() { }
}
