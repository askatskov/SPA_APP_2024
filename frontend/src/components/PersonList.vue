<template>
    <section class="container">
        <h2>Isikute Halduse Vorm</h2>

        <form @submit.prevent="submitForm">
            <label>
                Nimi:
                <input v-model="formData.name" required />
            </label>
            <label>
                Linn:
                <input v-model="formData.city" required />
            </label>
            <label>
                Piirkond:
                <input v-model="formData.region" required />
            </label>
            <label>
                Kuupäev:
                <input v-model="formData.date" type="date" required />
            </label>
            <button type="submit">{{ isEditing ? 'Uuenda' : 'Lisa' }}</button>
            <button v-if="isEditing" type="button" @click="cancelEdit">Tühista</button>
        </form>

        <hr />

        <div v-if="people.length > 0">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nimi</th>
                        <th>Linn</th>
                        <th>Piirkond</th>
                        <th>Kuupäev</th>
                        <th>Valikud</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="person in people" :key="person.id">
                        <td>{{ person.id }}</td>
                        <td>{{ person.name }}</td>
                        <td>{{ person.city }}</td>
                        <td>{{ person.region }}</td>
                        <td>{{ person.date }}</td>
                        <td>
                            <button @click="startEdit(person)">Muuda</button>
                            <button @click="removePerson(person.id)">Kustuta</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-else>
            <p>Isikuid pole lisatud.</p>
        </div>
    </section>
</template>

<script setup lang="ts">
    import { ref, onMounted } from "vue";
    import { usePersonsStore } from "@/stores/personsStore";
    import { storeToRefs } from "pinia";

    interface Person {
        id: number;
        name: string;
        city: string;
        region: string;
        date: string;
    }

    const store = usePersonsStore();
    const { persons: people } = storeToRefs(store);

    const formData = ref<Omit<Person, "id"> & { id?: number }>({
        id: undefined,
        name: "",
        city: "",
        region: "",
        date: ""
    });

    const isEditing = ref(false);

    onMounted(() => {
        store.load();
    });

    const resetForm = () => {
        formData.value = {
            id: undefined,
            name: "",
            city: "",
            region: "",
            date: ""
        };
    };

    const submitForm = async () => {
        if (isEditing.value) {
            await store.updatePerson({ ...formData.value } as Person);
            isEditing.value = false;
        } else {
            await store.addPerson({ ...formData.value });
        }
        resetForm();
    };

    const startEdit = (person: Person) => {
        formData.value = { ...person };
        isEditing.value = true;
    };

    const cancelEdit = () => {
        resetForm();
        isEditing.value = false;
    };

    const removePerson = async (id: number) => {
        await store.deletePerson(id);
    };
</script>

<style scoped>
    .container {
        max-width: 600px;
        margin: auto;
        padding: 1em;
    }

    form {
        display: flex;
        flex-direction: column;
        gap: 0.5em;
    }

    table {
        width: 100%;
        border-collapse: collapse;
    }

    th,
    td {
        border: 1px solid #ddd;
        padding: 0.5em;
        text-align: left;
    }

    button {
        margin-right: 0.5em;
    }
</style>
