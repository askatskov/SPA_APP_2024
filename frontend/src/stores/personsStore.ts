import { type Person } from "@/models/person";
import { ref } from "vue";
import { defineStore } from "pinia";
import useApi from "@/models/api";

export const usePersonsStore = defineStore('personsStore', () => {
    const apiGetPersons = useApi<Person[]>('persons');
    const persons = ref<Person[]>([]);
    let allPersons: Person[] = [];

    const load = async () => {
        await apiGetPersons.request();
        if (apiGetPersons.response.value) {
            allPersons = apiGetPersons.response.value;
            persons.value = allPersons;
        } else {
            allPersons = [];
            persons.value = [];
        }
    };

    const getPersonById = (id: number) => {
        return allPersons.find((person) => person.id === id);
    };

    const addPerson = async (person: Omit<Person, 'id'>) => {
        const apiAddPerson = useApi<Person>('persons', {
            method: 'POST',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(person),
        });

        await apiAddPerson.request();
        if (apiAddPerson.response.value) {
            await load();
        }
    };

    const updatePerson = async (person: Person) => {
        const apiUpdatePerson = useApi<Person>('persons/' + person.id, {
            method: 'PUT',
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(person),
        });

        await apiUpdatePerson.request();
        if (apiUpdatePerson.response.value) {
            await load();
        }
    };

    const deletePerson = async (id: number) => {
        try {
            const res = await fetch(`persons/${id}`, {
                method: 'DELETE',
                headers: {
                    Accept: 'application/json',
                },
            });

            if (res.ok) {
                await load();
            } else {
                console.error('Kustutamine ebaõnnestus', res.status);
            }
        } catch (error) {
            console.error('Viga kustutamisel', error);
        }
    };



    return { persons, load, getPersonById, addPerson, updatePerson, deletePerson };
});
