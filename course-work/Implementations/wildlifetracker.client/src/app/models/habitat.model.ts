export interface CreateHabitatDto {
    name: string;
    location: string;
    size?: number;
    climate: string;
    averageTemperature?: number;
}

export interface UpdateHabitatDto extends CreateHabitatDto {}

export interface Habitat extends CreateHabitatDto {
    id: number;
} 