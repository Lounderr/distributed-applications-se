export interface CreateSightingDto {
    notes?: string;
    weatherConditions: string;
    sightingDateTime: string;
    animalId: number;
    habitatId: number;
    observerId: string;
}

export interface UpdateSightingDto extends CreateSightingDto {}

export interface Sighting extends CreateSightingDto {
    id: number;
} 