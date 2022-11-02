<template>
  <div class="card bg-light p-3 submit-form">
    <h4 class="text-center">Cadastro de Estudante</h4>
    <div class="form-group">
      <label for="nome">Nome <span class="text-danger">*</span></label>
      <input
        type="text"
        class="form-control"
        id="nome"
        required
        v-model.trim="student.nome"
        name="nome"
        @keyup="student.nome.length < 3 ? null : keyupNome()"
        :class="{ 'is-invalid': !nomeValido }"
      />
      <div class="invalid-feedback">
        <span v-if="!nomeValido">Nome deve conter 3 ou mais caracteres.</span>
      </div>
    </div>

    <div class="form-group">
      <label for="CPF">CPF <span class="text-danger">*</span></label>
      <input
        class="form-control"
        id="CPF"
        required
        v-model="student.cpf"
        name="CPF"
        maxlength="14"
        @keyup="keyupCPF($event)"
        :class="{ 'is-invalid': !cpfValido }"
      />
      <div class="invalid-feedback">
        <span v-if="!cpfValido">CPF inválido.</span>
      </div>
    </div>

    <div class="form-group">
      <label for="email">E-mail</label>
      <input
        class="form-control"
        id="email"
        required
        v-model="student.email"
        name="email"
        @keyup="!emailValido ? keyupEmail() : null"
        :class="{ 'is-invalid': !emailValido }"
      />
      <div class="invalid-feedback">
        <span v-if="!emailValido">E-mail inválido.</span>
      </div>
    </div>

    <div class="row pl-3 pr-3 justify-content-between">
      <router-link :to="'/home'" class="btn btn-danger mr-2"
        >Voltar</router-link
      >
      <button @click="saveStudent" class="btn btn-success">Cadastrar</button>
    </div>
  </div>
</template>
  
  <script lang="ts">
import { defineComponent } from "vue";
import Swal from "sweetalert2/dist/sweetalert2.js";
import RegistrationDataService from "@/services/RegistrationDataService";
import Student from "@/types/Student";
import ResponseData from "@/types/ResponseData";

const regexpEmail =
  /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

const swalWithBootstrapButtons = Swal.mixin({
  customClass: {
    confirmButton: "btn btn-success",
    cancelButton: "btn btn-primary mr-3",
  },
  buttonsStyling: false,
});

export default defineComponent({
  name: "add-student",
  data() {
    return {
      student: {
        nome: "",
        cpf: "",
        email: "",
        ra: "",
        hashId: "",
      } as Student,
      nomeValido: true,
      cpfValido: true,
      emailValido: true,
    };
  },
  methods: {
    saveStudent() {
      if (this.validate())
        RegistrationDataService.create(this.student)
          .then((response: ResponseData) => {
            console.log(response.data);
            this.successSaveStudent();
          })
          .catch((e: Error) => {
            console.log(e);
          });
    },

    successSaveStudent() {
      swalWithBootstrapButtons
        .fire({
          title: "Sucesso",
          text: "Estudante cadastrado!",
          icon: "success",
          showCancelButton: true,
          confirmButtonText: "Novo Cadastro",
          cancelButtonText: "Ir para home",
          reverseButtons: true,
        })
        .then(async (result) => {
          if (result.isConfirmed) {
            this.newStudent();
          } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
          ) {
            this.$router.push({ name: "home" });
          }
        });
    },

    newStudent() {
      this.student = {
        nome: "",
        cpf: "",
        email: "",
        ra: "",
        hashId: "",
      } as Student;
    },

    keyupCPF(v: any) {
      this.student.cpf = this.student.cpf.replace(/\D/g, "");
      this.student.cpf = this.student.cpf.replace(/(\d{3})(\d)/, "$1.$2");
      this.student.cpf = this.student.cpf.replace(/(\d{3})(\d)/, "$1.$2");
      this.student.cpf = this.student.cpf.replace(/(\d{3})(\d{1,2})$/, "$1-$2");

      if (this.student.cpf.length == 14) this.cpfValido = true;
      if (this.student.cpf.length > 14) this.cpfValido = false;

      return this.student.cpf;
    },

    keyupNome() {
      this.student.nome.trim().length < 3
        ? (this.nomeValido = false)
        : (this.nomeValido = true);
    },

    keyupEmail() {
      !regexpEmail.test(this.student.email)
        ? (this.emailValido = false)
        : (this.emailValido = true);
    },

    validate(): boolean {
      this.keyupNome();
      this.keyupEmail();

      this.student.cpf.length != 14
        ? (this.cpfValido = false)
        : (this.cpfValido = true);

      return this.nomeValido && this.cpfValido && this.emailValido;
    },
  },
});
</script>
  
  <style>
.submit-form {
  max-width: 400px;
  margin: auto;
}
</style>