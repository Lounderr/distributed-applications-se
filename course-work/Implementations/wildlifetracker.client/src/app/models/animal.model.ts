export interface CreateAnimalDto {
    name: string;
    species: string;
    age: number;
    weight: number;
    height: number;
}

export interface UpdateAnimalDto extends CreateAnimalDto {}

export interface Animal extends CreateAnimalDto {
    id: number;
} 