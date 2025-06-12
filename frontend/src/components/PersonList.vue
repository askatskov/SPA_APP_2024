<template>
    <div class="about">
        <div v-if="!token" class="login-form">
            <h2>Login</h2>
            <input v-model="username" placeholder="Username" required />
            <input v-model="password" type="password" placeholder="Password" required />
            <button @click="login">Login</button>
        </div>

        <div v-else>
            <h1>Personlist</h1>

            <form @submit.prevent="addPerson">
                <input v-model="newPerson.name" placeholder="Nimi" required />
                <input v-model="newPerson.city" placeholder="Linn" required />
                <input v-model="newPerson.region" placeholder="Regioon" required />
                <input v-model="newPerson.date" type="date" required />
                <button type="submit">Lisa inimene</button>
            </form>

            <div class="personlist">
                <DataTable :value="persons">
                    <Column field="id" header="ID" />
                    <Column field="name" header="Name" />
                    <Column field="city" header="Linn" />
                    <Column field="region" header="Regioon" />
                    <Column field="date" header="Kuupäev" />
                    <Column header="Actions">
                        <template #body="{ data }">
                            <button @click="deletePerson(data.id)">Kustuta</button>
                            <button @click="editPerson(data)">Edit</button>
                        </template>
                    </Column>
                </DataTable>
            </div>
            <div>
                <h2>Logout</h2>
                <button @click="logout">Logout</button>
            </div>
            <div v-if="editingPerson" class="edit-form">
                <h2>Muuda isikut</h2>
                <form @submit.prevent="updatePerson">
                    <input v-model="editingPerson.name" placeholder="Nimi" required />
                    <input v-model="editingPerson.city" placeholder="Linn" required />
                    <input v-model="editingPerson.region" placeholder="Regioon" required />
                    <input v-model="editingPerson.date" type="date" required />
                    <button type="submit">Salvesta</button>
                    <button type="button" @click="editingPerson = null">Tühista</button>
                </form>
            </div>
        </div>
    </div>

</template>

<script setup lang="ts">

    import { ref, onMounted } from 'vue';
    import DataTable from 'primevue/datatable';
    import Column from 'primevue/column';

    interface Person {
        id: number;
        name: string;
        city: string;
        region: string;
        date: string;
    }

    const persons = ref<Person[]>([

    ]);

    const newPerson = ref<Omit<Person, 'id'>>({
        name: "",
        city: "",
        region: "",
        date: ""
    });

    const editingPerson = ref<Person | null>(null);

    const addPerson = () => {
        persons.value.push({
            id: persons.value.length + 1,
            ...newPerson.value
        });
        newPerson.value = { name: "", city: "", region: "", date: "" };
        SavePersonsToLocalStorage();
    };

    const deletePerson = (id: number) => {
        persons.value = persons.value.filter(p => p.id !== id);
        SavePersonsToLocalStorage();

    };

    const editPerson = (person: Person) => {
        editingPerson.value = { ...person };
    };

    const updatePerson = () => {
        if (!editingPerson.value) return;
        const index = persons.value.findIndex(p => p.id === editingPerson.value?.id);
        if (index !== -1) {
            persons.value[index] = { ...editingPerson.value };
        }
        editingPerson.value = null;
        SavePersonsToLocalStorage();

    };

    const username = ref('')
    const password = ref('')
    const token = ref(localStorage.getItem('token'));

    onMounted(() => {
        if (token.value) {
            loadPersonsApi();
        }
    });

    const SavePersonsToLocalStorage = () => {
        localStorage.setItem('persons', JSON.stringify(persons.value));
    };

    const loadPersonsFromLocalStorage = () => {
        const saved = localStorage.getItem('persons');
        if (saved) {
            persons.value = JSON.parse(saved);
        }
    };

    const login = async () => {
        const response = await fetch('https://localhost:7295/api/auth/login',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    username: username.value,
                    password: password.value
                })
            });

        if (response.ok) {
            const data = await response.json();
            localStorage.setItem('token', data.token);
            token.value = data.token;

            await loadPersonsApi();
        } else {
            alert("IM GONNA CRASHOUT AAAAAAAAAAAAAAAA");
        }
    };
    const loadPersonsApi = async () => {
        const response = await fetch('http://localhost:7295/api/events', {
            headers: {
                'Authorization': `Bearer ${token.value}`,
            }
        });

        if (response.ok) {
            const data = await response.json();
            persons.value = data.persons ?? [];
        }
        else {
            localStorage.removeItem('token')
            token.value = null;
        }
    }
    const logout = () => {
        localStorage.removeItem('token');
        token.value = null;
    };


</script>